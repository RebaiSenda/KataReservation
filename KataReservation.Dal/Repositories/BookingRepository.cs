using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KataReservation.Dal.Repositories;

public class BookingRepository(KataReservationContext kataReservation) : IBookingRepository
{


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
    public async Task<BookingRepositoryDto> GetBookingByIdAsync(int id)
    {
        var entity = await kataReservation.Bookings.SingleOrDefaultAsync(b => b.Id == id);
        if (entity is null)
        {
            return null!;
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


    public async Task<BookingRepositoryDto> AddBookingAsync(BookingRepositoryDto booking)
    {
        return await CreateBookingAsync(booking);
    }
}