using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KataReservation.Dal.Repositories;

public class BookingRepository(KataReservationContext kataReservation) : IBookingRepository
{
    public async Task<BookingRepositoryDto> CreateBookingAsync(BookingRepositoryDto booking)
    {
        var entity = new BookingEntity
        {
            RoomId = booking.RoomId,
            PersonId = booking.PersonId,
            BookingDate = booking.BookingDate.Date,
            StartSlot = booking.StartSlot,
            EndSlot = booking.EndSlot
        };

        kataReservation.Bookings.Add(entity);
        await kataReservation.SaveChangesAsync();

        return new BookingRepositoryDto(
            entity.Id,
            entity.RoomId,
            entity.PersonId,
            entity.BookingDate,
            entity.StartSlot,
            entity.EndSlot
        );
    }

    public async Task<bool> DeleteBookingAsync(int bookingId)
    {
        var booking = await kataReservation.Bookings.FindAsync(bookingId);
        if (booking == null)
        {
            return false;
        }

        kataReservation.Bookings.Remove(booking);
        await kataReservation.SaveChangesAsync();
        return true;
    }

    public async Task<BookingRepositoryDto?> GetBookingByIdAsync(int bookingId)
    {
        var booking = await kataReservation.Bookings.FindAsync(bookingId);
        if (booking == null)
        {
            return null;
        }

        return new BookingRepositoryDto(
            booking.Id,
            booking.RoomId,
            booking.PersonId,
            booking.BookingDate,
            booking.StartSlot,
            booking.EndSlot
        );
    }

    public async Task<IEnumerable<BookingRepositoryDto>> GetConflictingBookingsAsync(int roomId, DateTime bookingDate, int startSlot, int endSlot)
    {
        return await kataReservation.Bookings
            .Where(b => b.RoomId == roomId &&
                  b.BookingDate.Date == bookingDate.Date &&
                  ((b.StartSlot <= startSlot && b.EndSlot > startSlot) ||
                   (b.StartSlot < endSlot && b.EndSlot >= endSlot) ||
                   (b.StartSlot >= startSlot && b.EndSlot <= endSlot)))
            .Select(b => new BookingRepositoryDto(
                b.Id,
                b.RoomId,
                b.PersonId,
                b.BookingDate,
                b.StartSlot,
                b.EndSlot))
            .ToListAsync();
    }  
}