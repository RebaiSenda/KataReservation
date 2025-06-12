using KataReservation.MessagingService.Configuration;
using KataReservation.MessagingService.Interfaces;
using KataReservation.MessagingService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace KataReservation.MessagingService.Services
{
    public class RabbitMQService : IBookingMessagePublisher, IDisposable
    {
        private readonly RabbitMQConfig _config;
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly object _lockObject = new object();
        private readonly JsonSerializerOptions _jsonOptions;
        private bool _disposed = false;

        public RabbitMQService(IOptions<RabbitMQConfig> options, ILogger<RabbitMQService> logger)
        {
            _config = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configuration JSON optimisée
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            try
            {
                _connection = CreateConnection();
                _channel = _connection.CreateModel();

                // Configuration du channel selon les paramètres
                if (_config.ConfirmPublish)
                {
                    _channel.ConfirmSelect();
                }

                if (_config.EnablePublisherReturns)
                {
                    _channel.BasicReturn += OnBasicReturn;
                }

                InitializeInfrastructure();

                _logger.LogInformation("RabbitMQ service initialized successfully - Host: {Host}, Exchange: {Exchange}",
                    _config.HostName, _config.BookingExchange);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ service");
                Dispose();
                throw;
            }
        }

        private void OnBasicReturn(object sender, RabbitMQ.Client.Events.BasicReturnEventArgs e)
        {
            _logger.LogWarning("Message returned: {ReplyCode} - {ReplyText}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
                e.ReplyCode, e.ReplyText, e.Exchange, e.RoutingKey);
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                UserName = _config.Username,
                Password = _config.Password,
                Port = _config.Port,
                VirtualHost = _config.VirtualHost,
                RequestedHeartbeat = TimeSpan.FromSeconds(_config.RequestedHeartbeat),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(_config.NetworkRecoveryInterval),
                AutomaticRecoveryEnabled = _config.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = _config.TopologyRecoveryEnabled,
                DispatchConsumersAsync = true
            };

            return factory.CreateConnection($"BookingService_{Environment.MachineName}_{Guid.NewGuid():N}");
        }

        private void InitializeInfrastructure()
        {
            // Déclarer l'exchange principal
            DeclareExchangeWithRetry(_config.BookingExchange);

            // Déclarer les files d'attente
            var queueConfigurations = new[]
            {
                (_config.BookingCreatedQueue, _config.BookingCreatedRoutingKey),
                (_config.BookingDeletedQueue, _config.BookingDeletedRoutingKey),
                (_config.BookingUpdatedQueue, _config.BookingUpdatedRoutingKey)
            };

            foreach (var (queueName, routingKey) in queueConfigurations)
            {
                DeclareQueueWithRetry(queueName, _config.BookingExchange, routingKey);
            }
        }

        private void DeclareExchangeWithRetry(string exchangeName)
        {
            for (int retry = 0; retry < _config.MaxRetryAttempts; retry++)
            {
                try
                {
                    _channel.ExchangeDeclare(
                        exchange: exchangeName,
                        type: "direct",
                        durable: true,
                        autoDelete: false);

                    _logger.LogDebug("Exchange {ExchangeName} declared successfully", exchangeName);
                    return;
                }
                catch (Exception ex) when (retry < _config.MaxRetryAttempts - 1)
                {
                    var delay = _config.RetryDelayMilliseconds * (retry + 1);
                    _logger.LogWarning(ex, "Failed to declare exchange {ExchangeName}, retry {Retry}/{MaxRetries} in {Delay}ms",
                        exchangeName, retry + 1, _config.MaxRetryAttempts, delay);
                    Thread.Sleep(delay);
                }
            }
        }

        private void DeclareQueueWithRetry(string queueName, string exchangeName, string routingKey)
        {
            for (int retry = 0; retry < _config.MaxRetryAttempts; retry++)
            {
                try
                {
                    // Construire les arguments de la queue selon la configuration
                    Dictionary<string, object> arguments = null;

                    if (_config.MaxQueueLength.HasValue || _config.UseQuorumQueues || !string.IsNullOrEmpty(_config.QueueOverflowBehavior))
                    {
                        arguments = new Dictionary<string, object>();

                        if (_config.UseQuorumQueues)
                        {
                            arguments["x-queue-type"] = "quorum";
                        }

                        if (_config.MaxQueueLength.HasValue)
                        {
                            arguments["x-max-length"] = _config.MaxQueueLength.Value;
                            arguments["x-overflow"] = _config.QueueOverflowBehavior;
                        }
                    }

                    _channel.QueueDeclare(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: arguments);

                    _channel.QueueBind(
                        queue: queueName,
                        exchange: exchangeName,
                        routingKey: routingKey);

                    _logger.LogDebug("Queue {QueueName} declared and bound successfully", queueName);
                    return;
                }
                catch (Exception ex) when (retry < _config.MaxRetryAttempts - 1)
                {
                    var delay = _config.RetryDelayMilliseconds * (retry + 1);
                    _logger.LogWarning(ex, "Failed to declare queue {QueueName}, retry {Retry}/{MaxRetries} in {Delay}ms",
                        queueName, retry + 1, _config.MaxRetryAttempts, delay);
                    Thread.Sleep(delay);
                }
            }
        }

        public async Task PublishBookingCreatedAsync(BookingNotificationMessage message)
        {
            await PublishMessageAsync(message, _config.BookingExchange, _config.BookingCreatedRoutingKey, "BookingCreated");
        }

        public async Task PublishBookingDeletedAsync(BookingNotificationMessage message)
        {
            await PublishMessageAsync(message, _config.BookingExchange, _config.BookingDeletedRoutingKey, "BookingDeleted");
        }

        public async Task PublishBookingUpdatedAsync(BookingNotificationMessage message)
        {
            await PublishMessageAsync(message, _config.BookingExchange, _config.BookingUpdatedRoutingKey, "BookingUpdated");
        }

        private async Task PublishMessageAsync<T>(T message, string exchange, string routingKey, string messageType)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RabbitMQService));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var startTime = DateTime.UtcNow;
            var correlationId = Guid.NewGuid().ToString();

            try
            {
                var json = JsonSerializer.Serialize(message, _jsonOptions);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = _config.PersistentMessages;
                properties.ContentType = "application/json";
                properties.MessageId = correlationId;
                properties.CorrelationId = correlationId;
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                properties.Type = messageType;
                properties.AppId = "BookingService";

                // Headers enrichis pour le monitoring et le debugging
                properties.Headers = new Dictionary<string, object>
                {
                    { "source", "BookingService" },
                    { "version", "1.0" },
                    { "published_at", DateTime.UtcNow.ToString("O") },
                    { "environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown" },
                    { "machine_name", Environment.MachineName },
                    { "message_size", body.Length }
                };

                lock (_lockObject)
                {
                    if (_disposed) return;

                    _channel.BasicPublish(
                        exchange: exchange,
                        routingKey: routingKey,
                        basicProperties: properties,
                        body: body,
                        mandatory: _config.EnablePublisherReturns);
                }

                // Attendre la confirmation si activée
                if (_config.ConfirmPublish)
                {
                    var timeout = TimeSpan.FromSeconds(_config.MessageConfirmationTimeoutSeconds);
                    if (!_channel.WaitForConfirms(timeout))
                    {
                        _logger.LogWarning("Message confirmation timeout for {MessageType} after {Timeout}s - CorrelationId: {CorrelationId}",
                            messageType, timeout.TotalSeconds, correlationId);
                    }
                }

                var duration = DateTime.UtcNow - startTime;
                _logger.LogDebug("Published {MessageType} message successfully in {Duration}ms - CorrelationId: {CorrelationId}, Size: {Size} bytes",
                    messageType, duration.TotalMilliseconds, correlationId, body.Length);
            }
            catch (AlreadyClosedException ex)
            {
                _logger.LogError(ex, "Cannot publish {MessageType} message: connection is closed - CorrelationId: {CorrelationId}",
                    messageType, correlationId);
                throw new InvalidOperationException("RabbitMQ connection is closed", ex);
            }
            catch (Exception ex)
            {
                var duration = DateTime.UtcNow - startTime;
                _logger.LogError(ex, "Failed to publish {MessageType} message after {Duration}ms - CorrelationId: {CorrelationId}",
                    messageType, duration.TotalMilliseconds, correlationId);
                throw;
            }

            await Task.CompletedTask;
        }

        public Task PublishAsync<T>(T message)
        {
            _logger.LogWarning("Generic PublishAsync called - consider using specific methods for better performance and routing");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed) return;

            lock (_lockObject)
            {
                if (_disposed) return;
                _disposed = true;
            }

            try
            {
                if (_config.EnablePublisherReturns && _channel != null)
                {
                    _channel.BasicReturn -= OnBasicReturn;
                }

                _channel?.Close();
                _connection?.Close();

                _logger.LogInformation("RabbitMQ service closed gracefully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during RabbitMQ service disposal");
            }
            finally
            {
                _channel?.Dispose();
                _connection?.Dispose();
                _logger.LogInformation("RabbitMQ service disposed");
            }

            GC.SuppressFinalize(this);
        }

        ~RabbitMQService()
        {
            Dispose();
        }
    }
}