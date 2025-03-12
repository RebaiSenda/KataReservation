using System.ComponentModel.DataAnnotations;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Dtos.Repositories;

public record RoomRepositoryDto(int Id, string RoomName)
{
    public RoomRepositoryDto(RoomServiceDto value) :
        this(value.Id, value.RoomName)
    { }
}
public record RoomResponse(RoomServiceDto Room);
public record RoomsResponse(IEnumerable<RoomServiceDto> Rooms);
public record CreateRoomRequest([Required] string RoomName);
//{
    //public CreateRoomRequest()
    //{
    //}
//}

public record UpdateRoomRequest([Required] string RoomName);

public record RoomServiceDto(int Id, string RoomName)
{
    public RoomServiceDto(Room room) : this(room.Id, room.RoomName) { }
}

public record Room(int Id, string RoomName);