using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit.Abstractions;

namespace KataReservation.Tests
{
    public class RabbitMQTests : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ITestOutputHelper _output;
        private readonly List<string> _receivedMessages = new();
        private readonly ManualResetEvent _messageReceived = new(false);

        // Configurations RabbitMQ
        private const string HostName = "localhost";
        private const string Username = "guest";
        private const string Password = "guest";
        private const string Exchange = "booking_exchange";
        private const string Queue = "booking_created_queue";
        private const string RoutingKey = "booking.created";
        private const string TestQueue = "test_queue";

        public RabbitMQTests(ITestOutputHelper output)
        {
            _output = output;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = HostName,
                    UserName = Username,
                    Password = Password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Configuration de l'exchange et de la queue pour les tests
                _channel.ExchangeDeclare(
                    exchange: Exchange,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false);

                _channel.QueueDeclare(
                    queue: Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                _channel.QueueBind(
                    queue: Queue,
                    exchange: Exchange,
                    routingKey: RoutingKey);

                // Créer une queue spécifique pour les tests
                _channel.QueueDeclare(
                    queue: TestQueue,
                    durable: false,
                    exclusive: true,
                    autoDelete: true);

                _channel.QueueBind(
                    queue: TestQueue,
                    exchange: Exchange,
                    routingKey: RoutingKey);

                _output.WriteLine("Configuration RabbitMQ effectuée avec succès");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Erreur lors de la configuration RabbitMQ: {ex.Message}");
                throw;
            }
        }

        [Fact]
        public void CanConnectToRabbitMQ()
        {
            // Vérifie simplement que la connexion fonctionne
            Assert.NotNull(_connection);
            Assert.True(_connection.IsOpen);
            Assert.NotNull(_channel);
            Assert.True(_channel.IsOpen);
        }

        [Fact]
        public async Task CanSendAndReceiveBookingCreatedMessage()
        {
            // Arrangement
            var bookingId = new Random().Next(1, 10000);
            var roomId = 101;
            var personId = 202;

            // Configurer un consommateur pour le test
            SetupTestConsumer();

            // Action
            SendTestBookingCreatedMessage(bookingId, roomId, personId);

            // Assertion
            // Attendre la réception du message avec un timeout de 5 secondes
            var messageReceived = _messageReceived.WaitOne(TimeSpan.FromSeconds(5));
            Assert.True(messageReceived, "Le message n'a pas été reçu dans le délai imparti");

            Assert.Single(_receivedMessages);
            var receivedMessage = _receivedMessages[0];
            _output.WriteLine($"Message reçu: {receivedMessage}");

            // Vérifier que le message contient les informations envoyées
            Assert.Contains($"\"BookingId\":{bookingId}", receivedMessage);
            Assert.Contains($"\"RoomId\":{roomId}", receivedMessage);
            Assert.Contains($"\"PersonId\":{personId}", receivedMessage);
            Assert.Contains("\"Status\":\"Created\"", receivedMessage);
        }

        [Fact]
        public async Task CanSendAndReceiveBookingDeletedMessage()
        {
            // Arrangement
            var bookingId = new Random().Next(1, 10000);

            // Configurer un consommateur pour le test
            SetupTestConsumer();

            // Action
            SendTestBookingDeletedMessage(bookingId);

            // Assertion
            // Attendre la réception du message avec un timeout de 5 secondes
            var messageReceived = _messageReceived.WaitOne(TimeSpan.FromSeconds(5));
            Assert.True(messageReceived, "Le message n'a pas été reçu dans le délai imparti");

            Assert.Single(_receivedMessages);
            var receivedMessage = _receivedMessages[0];
            _output.WriteLine($"Message reçu: {receivedMessage}");

            // Vérifier que le message contient les informations envoyées
            Assert.Contains($"\"BookingId\":{bookingId}", receivedMessage);
            Assert.Contains("\"Status\":\"Deleted\"", receivedMessage);
        }

        [Fact]
        public void CanDeclareQueueAndExchange()
        {
            // Tentative de déclaration des ressources RabbitMQ - si elles existent déjà,
            // aucune exception ne devrait être levée
            Assert.NotNull(_channel);

            _channel.ExchangeDeclarePassive(Exchange);
            _channel.QueueDeclarePassive(Queue);

            // Si nous arrivons ici, c'est que les ressources existent et sont accessibles
            Assert.True(true);
        }

        private void SetupTestConsumer()
        {
            _receivedMessages.Clear();
            _messageReceived.Reset();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _output.WriteLine($"Message reçu dans le consommateur de test: {message}");
                _receivedMessages.Add(message);

                // Signaler qu'un message a été reçu
                _messageReceived.Set();

                // Acquitter la réception du message
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            // Commencer à consommer les messages
            _channel.BasicConsume(
                queue: TestQueue,
                autoAck: false,
                consumer: consumer);
        }

        private void SendTestBookingCreatedMessage(int bookingId, int roomId, int personId)
        {
            var message = new
            {
                BookingId = bookingId,
                RoomId = roomId,
                PersonId = personId,
                BookingDate = DateTime.Today,
                StartSlot = 9,
                EndSlot = 10,
                Status = "Created"
            };

            SendMessage(message);
            _output.WriteLine($"Message envoyé pour la création de réservation {bookingId}");
        }

        private void SendTestBookingDeletedMessage(int bookingId)
        {
            var message = new
            {
                BookingId = bookingId,
                Status = "Deleted"
            };

            SendMessage(message);
            _output.WriteLine($"Message envoyé pour la suppression de réservation {bookingId}");
        }

        private void SendMessage(object message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: Exchange,
                routingKey: RoutingKey,
                basicProperties: properties,
                body: body);
        }

        public void Dispose()
        {
            // Nettoyage des ressources
            try
            {
                // Ne pas supprimer la queue principale, mais seulement la queue de test
                _channel?.QueueDelete(TestQueue);

                _channel?.Close();
                _connection?.Close();

                _channel?.Dispose();
                _connection?.Dispose();
                _messageReceived?.Dispose();
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Erreur lors du nettoyage des ressources: {ex.Message}");
            }
        }
    }
}