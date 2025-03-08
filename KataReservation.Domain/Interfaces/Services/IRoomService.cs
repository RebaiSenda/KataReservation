using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using RoomServiceDto = KataReservation.Domain.Dtos.Services.RoomServiceDto;

namespace KataReservation.Domain.Interfaces.Services;

public interface IRoomService
{
    Task<RoomServiceDto> GetRoomByIdAsync(int id);
    Task<IEnumerable<RoomServiceDto>> GetRoomsAsync();

    Task<RoomServiceDto> CreateRoomAsync(string roomName);
    Task<RoomServiceDto> UpdateRoomAsync(int id, string roomName);
    Task<bool> DeleteRoomAsync(int id);
    //Task<RoomServiceDto> GetRoomByIdAsync(int id); // Add this method to the interface
}
