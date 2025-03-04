using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;

namespace KataReservation.Dal.Repositories;

public class RoomRepository : IRoomRepository
{
    private static readonly IList<RoomEntity> Rooms =
[
    new RoomEntity
        {
            Id = 1,
            RoomName= "Room 1"
        },
        new RoomEntity
        {
            Id = 2,
            RoomName= "Room 2"
        }
    ];

    public async Task<IEnumerable<RoomRepositoryDto>> GetRoomsAsync()
    {
        var values = await Task.FromResult(Rooms);

        return values.Select(v => new RoomRepositoryDto(v.Id, v.RoomName));

    }
}