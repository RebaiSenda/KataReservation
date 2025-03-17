using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;
using Room = KataReservation.Domain.Models.Room;
using RoomServiceDto = KataReservation.Domain.Dtos.Services.RoomServiceDto;

namespace KataReservation.Domain.Services;

public class RoomService(IRoomRepository roomRepository) : IRoomService
{
    public async Task<IEnumerable<RoomServiceDto>> GetRoomsAsync()
    {
        var roomsDtos = await roomRepository.GetRoomsAsync();
        var rooms = roomsDtos.Select(v => new Room(v.Id, v.RoomName));
        return rooms.Select(v => new RoomServiceDto(v));
    }
    public async Task<RoomServiceDto> GetRoomByIdAsync(int id)
    {
        var roomDto = await roomRepository.GetRoomByIdAsync(id);
        if (roomDto == null)
        {
            return null!;
        }
        var room = new Room(roomDto.Id, roomDto.RoomName);
        return new RoomServiceDto(room);
    }

    public async Task<RoomServiceDto> CreateRoomAsync(string roomName)
    {
        var newRoomDto = await roomRepository.CreateRoomAsync(roomName);
        var room = new Room(newRoomDto.Id, newRoomDto.RoomName);
        return new RoomServiceDto(room);
    }

    public async Task<RoomServiceDto> UpdateRoomAsync(int id, string roomName)
    {
        var updatedRoomDto = await roomRepository.UpdateRoomAsync(id, roomName);
        if (updatedRoomDto == null)
        {
            return null!
                ;
        }
        var room = new Room(updatedRoomDto.Id, updatedRoomDto.RoomName);
        return new RoomServiceDto(room);
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        return await roomRepository.DeleteRoomAsync(id);
    }
}

