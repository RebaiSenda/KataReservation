using System.ComponentModel.DataAnnotations;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Tests.Api.Dtos
{
    public class UpdateBookingRequest(int roomId, int personId, DateTime bookingDate, int startSlot, int endSlot)
    {
        [Required]
        public int RoomId { get; set; } = roomId;

        [Required]
        public int PersonId { get; set; } = personId;

        [Required]
        public DateTime BookingDate { get; set; } = bookingDate;

        [Required]
        public int StartSlot { get; set; } = startSlot;
        [Required]
        public int EndSlot { get; set; } = endSlot;

        public BookingServiceDto ToModel(int id) => new(id, RoomId, PersonId, BookingDate, StartSlot, EndSlot);
    }
}
