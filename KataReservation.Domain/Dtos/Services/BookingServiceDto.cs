using KataReservation.Domain.Models;

namespace KataReservation.Domain.Dtos.Services;

public record BookingServiceDto(int Id, int RoomId, int PersonId, DateTime BookingDate, int StartSlot, int EndSlot)
{
    internal BookingServiceDto(Booking value) : 
        this(value.Id, value.RoomId, value.PersonId, value.BookingDate, value.StartSlot, value.EndSlot) { }
}