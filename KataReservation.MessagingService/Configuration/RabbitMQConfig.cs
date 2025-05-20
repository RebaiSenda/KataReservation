namespace KataReservation.MessagingService.Configuration
{
    public class RabbitMQConfig
    {
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
    }
}
