using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Interfaces.Services;

public interface IRoomService
{
    Task<IEnumerable<RoomServiceDto>> GetRoomsAsync();
}