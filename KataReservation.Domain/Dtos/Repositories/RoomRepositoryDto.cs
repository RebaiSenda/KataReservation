using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Dtos.Repositories;

public record RoomRepositoryDto(int Id, string RoomName)
{
    public RoomRepositoryDto(RoomServiceDto value) :
        this(value.Id, value.RoomName)
    { }
}