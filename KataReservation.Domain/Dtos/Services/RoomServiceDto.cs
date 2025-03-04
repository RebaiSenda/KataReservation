using KataReservation.Domain.Models;

namespace KataReservation.Domain.Dtos.Services;

public record RoomServiceDto(int Id, string RoomName)
{
    internal RoomServiceDto(Room value) :
        this(value.Id, value.RoomName)
    { }
}