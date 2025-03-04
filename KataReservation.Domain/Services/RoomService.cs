using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Models;

namespace KataReservation.Domain.Services;

public class RoomService(IRoomRepository roomRepository) : IRoomService
{
    public async Task<IEnumerable<RoomServiceDto>> GetRoomsAsync()
    {
        var RoomsDtos = await roomRepository.GetRoomsAsync();
        var Rooms = RoomsDtos.Select(v => new Room(v.Id, v.RoomName));
        return Rooms.Select(v => new RoomServiceDto(v));
    }

}