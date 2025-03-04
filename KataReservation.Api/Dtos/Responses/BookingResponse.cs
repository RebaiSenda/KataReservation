using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Responses;

public record BookingResponse(int Id, int RoomId, int PersonId, DateTime BookingDate, int StartSlot, int EndSlot)
{
    public BookingResponse(BookingServiceDto value) : this(value.Id, value.RoomId, value.PersonId, value.BookingDate, value.StartSlot, value.EndSlot) { }
}