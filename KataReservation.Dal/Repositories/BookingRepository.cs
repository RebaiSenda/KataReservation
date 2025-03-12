using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KataReservation.Dal.Repositories;

public class BookingRepository(KataReservationContext kataReservation) : IBookingRepository
{
    //private readonly KataReservationContext _context = context;
    public async Task<BookingRepositoryDto?> GetBookingByIdAsync(int id)
    {
        var entity = await Task.FromResult(kataReservation.Bookings.SingleOrDefault(b => b.Id == id));
        if (entity is null)
        {
            return null;
        }
        return new BookingRepositoryDto(
            entity.Id,
            entity.RoomId,
            entity.PersonId,
            entity.BookingDate,
            entity.StartSlot,
            entity.EndSlot
        );
    }

    public async Task<IEnumerable<BookingRepositoryDto>> GetBookingsAsync()
    {
        var bookings = await Task.FromResult(kataReservation.Bookings.ToList());
        return bookings.Select(b => new BookingRepositoryDto(
            b.Id,
            b.RoomId,
            b.PersonId,
            b.BookingDate,
            b.StartSlot,
            b.EndSlot
        ));
    }

    public async Task<BookingRepositoryDto> CreateBookingAsync(BookingRepositoryDto value)
    {
        var newBooking = new BookingEntity
        {
            RoomId = value.RoomId,
            PersonId = value.PersonId,
            BookingDate = value.BookingDate,
            StartSlot = value.StartSlot,
            EndSlot = value.EndSlot
        };

        await Task.Run(() =>
        {
            kataReservation.Bookings.Add(newBooking);
            kataReservation.SaveChanges();
        });

        return new BookingRepositoryDto(
            newBooking.Id,
            newBooking.RoomId,
            newBooking.PersonId,
            newBooking.BookingDate,
            newBooking.StartSlot,
            newBooking.EndSlot
        );
    }

    public async Task<BookingRepositoryDto?> UpdateBookingAsync(BookingRepositoryDto value)
    {
        var entity = await Task.Run(() =>
        {
            var booking = kataReservation.Bookings.SingleOrDefault(b => b.Id == value.Id);
            if (booking is not null)
            {
                booking.RoomId = value.RoomId;
                booking.PersonId = value.PersonId;
                booking.BookingDate = value.BookingDate;
                booking.StartSlot = value.StartSlot;
                booking.EndSlot = value.EndSlot;

                kataReservation.SaveChanges();
            }
            return booking;
        });

        if (entity is null)
        {
            return null;
        }

        return new BookingRepositoryDto(
            entity.Id,
            entity.RoomId,
            entity.PersonId,
            entity.BookingDate,
            entity.StartSlot,
            entity.EndSlot
        );
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        return await Task.Run(() =>
        {
            var booking = kataReservation.Bookings.SingleOrDefault(b => b.Id == id);
            if (booking is null)
            {
                return false;
            }

            kataReservation.Bookings.Remove(booking);
            kataReservation.SaveChanges();
            return true;
        });
    }


    public async Task<IEnumerable<BookingRepositoryDto>> GetAllBookingsAsync()
    {
        var bookingEntities = await kataReservation.Bookings
            .AsNoTracking()
            .ToListAsync();

        return bookingEntities.Select(MapEntityToDto);
    }
    public async Task<IEnumerable<BookingRepositoryDto>> GetBookingsByDateAsync(DateTime date)
    {
        var bookings = await Task.FromResult(
            kataReservation.Bookings
                .Where(b => b.BookingDate.Date == date.Date)
                .ToList()
        );

        return bookings.Select(b => new BookingRepositoryDto(
            b.Id,
            b.RoomId,
            b.PersonId,
            b.BookingDate,
            b.StartSlot,
            b.EndSlot
        ));
    }

    public async Task<IEnumerable<BookingRepositoryDto>> GetBookingsByRoomAndDateAsync(int roomId, DateTime date)
    {
        var bookings = await Task.FromResult(
            kataReservation.Bookings
                .Where(b => b.RoomId == roomId && b.BookingDate.Date == date.Date)
                .ToList()
        );

        return bookings.Select(b => new BookingRepositoryDto(
            b.Id,
            b.RoomId,
            b.PersonId,
            b.BookingDate,
            b.StartSlot,
            b.EndSlot
        ));
    }
    public async Task<List<BookingRepositoryDto>> GetByRoomAndDateAsync(int roomId, DateTime date)
    {

        var bookings = await kataReservation.Bookings
            .Include(b => b.Room)
            .Include(b => b.Person)
            .Where(b => b.RoomId == roomId && b.BookingDate.Date == date.Date)
            .ToListAsync();

        return bookings.Select(b => new BookingRepositoryDto(
            b.Id,
            b.RoomId, 
            b.PersonId, 
            b.BookingDate,
            b.StartSlot,
            b.EndSlot
        )).ToList();
    }
    public async Task<BookingRepositoryDto> AddBookingAsync(BookingRepositoryDto booking)
    {
        return await CreateBookingAsync(booking);
    }
    private static BookingRepositoryDto MapEntityToDto(BookingEntity entity)
    {
        return new BookingRepositoryDto(
            entity.Id,
            entity.RoomId,
            entity.PersonId,
            entity.BookingDate,
            entity.StartSlot,
            entity.EndSlot
        );
    }
}