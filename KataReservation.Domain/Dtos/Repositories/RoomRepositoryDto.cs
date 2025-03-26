using System.ComponentModel.DataAnnotations;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Dtos.Repositories;

public record RoomRepositoryDto(int Id, string RoomName)
{

}

public record RoomServiceDto(int Id, string RoomName)
{
    public RoomServiceDto(Room room) : this(room.Id, room.RoomName) { }
}

public record Room(int Id, string RoomName);