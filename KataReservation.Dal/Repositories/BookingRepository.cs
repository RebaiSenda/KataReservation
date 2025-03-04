using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;

namespace KataReservation.Dal.Repositories;

public class BookingRepository : IBookingRepository
{
    private static readonly IList<BookingEntity> Values =
    [
        new BookingEntity
        {
            Id = 100,
            RoomId = 1,
            PersonId = 1,
            BookingDate = DateTime.Now,
            StartSlot = 1,
            EndSlot= 1
        },
        new BookingEntity
        {
            Id = 100,
            RoomId = 1,
            PersonId = 1,
            BookingDate = DateTime.Now,
            StartSlot = 1,
            EndSlot= 1
        }
    ];

    public async Task<BookingRepositoryDto?> GetValueByIdAsync(int id)
    {
        var entity = await Task.FromResult(Values.SingleOrDefault(v => v.Id == id));

        if (entity is null)
        {
            return null;
        }

        return new BookingRepositoryDto(entity.Id, entity.RoomId,entity.PersonId,entity.BookingDate,entity.StartSlot,entity.EndSlot);
    }

    public async Task<IEnumerable<BookingRepositoryDto>> GetValuesAsync()
    {
        var values = await Task.FromResult(Values);

        return values.Select(v => new BookingRepositoryDto(v.Id, v.RoomId, v.PersonId, v.BookingDate, v.StartSlot, v.EndSlot));
    }

    public async Task<BookingRepositoryDto> CreateValueAsync(BookingRepositoryDto value)
    {
        await Task.Run(() => Values.Add(new BookingEntity
        {
            Id = value.Id,
            RoomId = value.RoomId,
            PersonId = value.PersonId,
            BookingDate = value.BookingDate,
            StartSlot = value.StartSlot
        }));

        return value;
    }

    public async Task<BookingRepositoryDto?> UpdateValueAsync(BookingRepositoryDto value)
    {
        var entity = await Task.Run(() =>
        {
            var entity = Values.SingleOrDefault(v => v.Id == value.Id);

            if (entity is not null)
            {
                entity.StartSlot = value.StartSlot;
                entity.EndSlot = value.EndSlot;
                entity.BookingDate = value.BookingDate;
                entity.PersonId = value.PersonId;
                entity.RoomId = value.RoomId;
                entity.Id = value.Id;
            }

            return entity;
        });

        if (entity is null)
        {
            return null;
        }

        return new BookingRepositoryDto(value.Id, value.RoomId, value.PersonId, value.BookingDate, value.StartSlot, value.EndSlot);
    }

    public async Task<bool> DeleteValueAsync(int id) =>
        await Task.Run(() =>
        {
            var entity = Values.SingleOrDefault(v => v.Id == id);

            if (entity is null)
            {
                return false;
            }

            Values.Remove(entity);
            return true;
        });
}