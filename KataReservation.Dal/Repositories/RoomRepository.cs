using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace KataReservation.Dal.Repositories;

public class RoomRepository(KataReservationContext kataReservation) : IRoomRepository 
{

    public async Task<IEnumerable<RoomRepositoryDto>> GetRoomsAsync()
    {
        
        var values = await kataReservation.Rooms.ToListAsync();
        return values.Select(v => new RoomRepositoryDto(v.Id, v.RoomName));
    }
    public async Task<RoomRepositoryDto> GetRoomByIdAsync(int id)
    {

        var room = await kataReservation.Rooms.FirstOrDefaultAsync(r => r.Id == id);

        if (room == null)
        {
            return null;
        }

        return new RoomRepositoryDto(room.Id, room.RoomName);

    }



    public async Task<RoomRepositoryDto> CreateRoomAsync(string roomName)
    {
        var roomEntity = new RoomEntity { RoomName = roomName };
        kataReservation.Rooms.Add(roomEntity);
        await kataReservation.SaveChangesAsync();

        return new RoomRepositoryDto(roomEntity.Id, roomEntity.RoomName);
    }

    public async Task<RoomRepositoryDto> UpdateRoomAsync(int id, string roomName)
    {
        var roomEntity = await kataReservation.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (roomEntity == null)
        {
            return null;
        }

        roomEntity.RoomName = roomName;
        await kataReservation.SaveChangesAsync();

        return new RoomRepositoryDto(roomEntity.Id, roomEntity.RoomName);
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var roomEntity = await kataReservation.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (roomEntity == null)
        {
            return false;
        }

        kataReservation.Rooms.Remove(roomEntity);
        await kataReservation.SaveChangesAsync();

        return true;
    }
}