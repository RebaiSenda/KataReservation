using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Models;

namespace KataReservation.Domain.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{

    public async Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto booking)
    {
        await ValidateBookingAsync(booking);

        var repositoryDto = new BookingRepositoryDto(booking);
        var result = await bookingRepository.CreateBookingAsync(repositoryDto);

        return new BookingServiceDto(
            result.Id,
            result.RoomId,
            result.PersonId,
            result.BookingDate,
            result.StartSlot,
            result.EndSlot
        );
    }

    public async Task<bool> DeleteBookingAsync(int bookingId)
    {
        return await bookingRepository.DeleteBookingAsync(bookingId);
    }

    private async Task ValidateBookingAsync(BookingServiceDto booking)
    {
        if (booking.BookingDate.Date < DateTime.Today)
        {
            throw new InvalidOperationException("La date de réservation doit être une date future.");
        }

        if (booking.StartSlot >= booking.EndSlot)
        {
            throw new InvalidOperationException("L'heure de début doit être avant l'heure de fin.");
        }

        if (booking.StartSlot < 1 || booking.StartSlot > 24 || booking.EndSlot < 1 || booking.EndSlot > 24)
        {
            throw new InvalidOperationException("Les créneaux horaires doivent être entre 1 et 24.");
        }

        var conflictingBookings = await bookingRepository.GetConflictingBookingsAsync(
            booking.RoomId,
            booking.BookingDate,
            booking.StartSlot,
            booking.EndSlot
        );

        if (conflictingBookings.Any())
        {
            throw new InvalidOperationException("Ce créneau horaire est déjà réservé pour cette salle.");
        }
    }
}