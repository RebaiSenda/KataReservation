namespace KataReservation.MessagingService.Configuration
{
    public class RabbitMQConfig
    {
        // Configuration de base (votre configuration actuelle)
        public string HostName { get; set; } = "localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";

        // Exchange principal
        public string BookingExchange { get; set; } = "booking_exchange";

        // Files d'attente
        public string BookingCreatedQueue { get; set; } = "booking_created_queue";
        public string BookingDeletedQueue { get; set; } = "booking_deleted_queue";
        public string BookingUpdatedQueue { get; set; } = "booking_updated_queue";

        // Clés de routage
        public string BookingCreatedRoutingKey { get; set; } = "booking.created";
        public string BookingDeletedRoutingKey { get; set; } = "booking.deleted";
        public string BookingUpdatedRoutingKey { get; set; } = "booking.updated";

        // Extensions optionnelles pour de meilleures performances
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public int RequestedHeartbeat { get; set; } = 30;
        public int NetworkRecoveryInterval { get; set; } = 10;
        public bool AutomaticRecoveryEnabled { get; set; } = true;
        public bool TopologyRecoveryEnabled { get; set; } = true;
        public int MessageConfirmationTimeoutSeconds { get; set; } = 5;
        public int MaxRetryAttempts { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 100;

        // Optimisations des queues
        public int? MaxQueueLength { get; set; } = null; // null = pas de limite
        public string QueueOverflowBehavior { get; set; } = "drop-head"; // "drop-head", "reject-publish"
        public bool UseQuorumQueues { get; set; } = false; // Nécessite RabbitMQ 3.8+

        // Paramètres de performance
        public bool PersistentMessages { get; set; } = true;
        public bool ConfirmPublish { get; set; } = true;
        public bool EnablePublisherReturns { get; set; } = false;
    }
}