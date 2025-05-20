using KataReservation.MessagingService.Configuration;
using KataReservation.MessagingService.Interfaces;
using KataReservation.MessagingService.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace KataReservation.MessagingService.Services
{
    public class RabbitMQService : IMessagePublisher, IDisposable
    {
        private readonly RabbitMQConfig _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(IOptions<RabbitMQConfig> options)
        {
            _config = options.Value;
            // Créer une connexion à RabbitMQ
            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                UserName = _config.Username,
                Password = _config.Password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();  // Correction: _connection au lieu de *connection

            // Déclarer l'exchange principal
            _channel.ExchangeDeclare(
                exchange: _config.BookingExchange,
                type: "direct",
                durable: true,
                autoDelete: false);

            // Déclarer les files d'attente
            DeclareQueue(_config.BookingCreatedQueue, _config.BookingExchange, _config.BookingCreatedRoutingKey);  // Correction: _config au lieu de *config
            DeclareQueue(_config.BookingDeletedQueue, _config.BookingExchange, _config.BookingDeletedRoutingKey);
            DeclareQueue(_config.BookingUpdatedQueue, _config.BookingExchange, _config.BookingUpdatedRoutingKey);
        }

        private void DeclareQueue(string queueName, string exchangeName, string routingKey)
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);
        }

        public Task PublishBookingCreatedAsync(BookingNotificationMessage message)
        {
            PublishMessage(message, _config.BookingExchange, _config.BookingCreatedRoutingKey);  // Correction: _config au lieu de *config
            return Task.CompletedTask;
        }

        public Task PublishBookingDeletedAsync(BookingNotificationMessage message)
        {
            PublishMessage(message, _config.BookingExchange, _config.BookingDeletedRoutingKey);
            return Task.CompletedTask;
        }

        public Task PublishBookingUpdatedAsync(BookingNotificationMessage message)
        {
            PublishMessage(message, _config.BookingExchange, _config.BookingUpdatedRoutingKey);
            return Task.CompletedTask;
        }

        private void PublishMessage<T>(T message, string exchange, string routingKey)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        // Implémentation vide de la méthode de l'interface Foundatio.Messaging.IMessagePublisher
        public Task PublishAsync<T>(T message)
        {
            // Cette méthode n'est pas utilisée dans votre application, mais est nécessaire
            // pour satisfaire l'interface Foundatio.Messaging.IMessagePublisher
            return Task.CompletedTask;
        }
    }
}