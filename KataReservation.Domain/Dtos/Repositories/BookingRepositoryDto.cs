using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Dtos.Repositories;

public record BookingRepositoryDto(int Id, int RoomId, int PersonId, DateTime BookingDate,int StartSlot,int EndSlot)
{
    public BookingRepositoryDto(BookingServiceDto value) : 
        this(value.Id, value.RoomId, value.PersonId, value.BookingDate, value.StartSlot, value.EndSlot) { }
}