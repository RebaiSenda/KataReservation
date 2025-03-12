using System.ComponentModel.DataAnnotations;

namespace KataReservation.Api.Dtos.Requests
{
    public record CheckBookingRequest
    {
        [Required]
        public int RoomId { get; init; }

        [Required]
        public DateTime BookingDate { get; init; }

        [Required]
        [Range(0, 23)]
        public int StartSlot { get; init; }

        [Required]
        [Range(1, 24)]
        public int EndSlot { get; init; }
    }
}
