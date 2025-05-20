using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Exceptions;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;

namespace KataReservation.Domain.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    public async Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto booking)
    {
        try
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
        catch (InvalidOperationException ex) when (ex.Message.Contains("créneau horaire"))
        {
            // Si l'exception est due à un conflit de réservation, récupérer les créneaux disponibles
            var availableSlots = await GetAvailableSlotsForDay(booking.RoomId, booking.BookingDate);
            throw new BookingConflictException(
                "Ce créneau horaire est déjà réservé pour cette salle. Voici les créneaux disponibles pour cette journée.",
                availableSlots
            );
        }
    }
    public async Task<BookingServiceDto?> GetBookingAsync(int bookingId)
    {
        // Call the repository method to get the booking
        var booking = await bookingRepository.GetBookingByIdAsync(bookingId);

        // If no booking is found, return null
        if (booking == null)
        {
            return null;
        }

        // Map the repository DTO to service DTO
        return new BookingServiceDto(
            booking.Id,
            booking.RoomId,
            booking.PersonId,
            booking.BookingDate,
            booking.StartSlot,
            booking.EndSlot
        );
    }
    public async Task<List<SlotDto>> GetAvailableSlotsForDay(int roomId, DateTime date)
    {
        // Récupérer toutes les réservations pour cette salle et cette date
        var existingBookings = await bookingRepository.GetBookingsByRoomAndDateAsync(roomId, date);

        // Supposons que les créneaux valides sont de 1 à 24 (pour simplifier)
        var allSlots = Enumerable.Range(1, 24);

        // Créer une liste de tous les créneaux horaires occupés
        var occupiedSlots = new HashSet<int>();
        foreach (var booking in existingBookings)
        {
            // Ajouter tous les créneaux entre StartSlot et EndSlot
            for (int slot = booking.StartSlot; slot < booking.EndSlot; slot++)
            {
                occupiedSlots.Add(slot);
            }
        }

        // Déterminer les créneaux libres
        var availableSlots = new List<SlotDto>();
        int? startSlot = null;

        foreach (var slot in allSlots)
        {
            if (!occupiedSlots.Contains(slot))
            {
                // Si on n'a pas encore commencé un créneau libre, on commence maintenant
                if (startSlot == null)
                {
                    startSlot = slot;
                }
            }
            else
            {
                // Si on avait commencé un créneau libre et qu'on trouve un créneau occupé, 
                // on ajoute le créneau libre à la liste
                if (startSlot != null)
                {
                    availableSlots.Add(new SlotDto(startSlot.Value, slot));
                    startSlot = null;
                }
            }
        }

        // Si on finit sur un créneau libre, on l'ajoute aussi
        if (startSlot != null)
        {
            availableSlots.Add(new SlotDto(startSlot.Value, 25)); // 25 représente la fin de la journée
        }

        return availableSlots;
    }

    public async Task<bool> DeleteBookingAsync(int bookingId)
    {
        return await bookingRepository.DeleteBookingAsync(bookingId);
    }
    public async Task<BookingServiceDto?> UpdateBookingAsync(BookingServiceDto booking)
    {
        // Vérifier si la réservation existe
        var existingBooking = await bookingRepository.GetBookingByIdAsync(booking.Id);
        if (existingBooking == null)
        {
            return null; // Retourne null si la réservation n'existe pas
        }

        try
        {
            // Valider la nouvelle réservation (mêmes règles que pour la création)
            await ValidateBookingAsync(booking);

            // Convertir en DTO pour le repository
            var repositoryDto = new BookingRepositoryDto(
                booking.Id,
                booking.RoomId,
                booking.PersonId,
                booking.BookingDate,
                booking.StartSlot,
                booking.EndSlot
            );

            // Mettre à jour la réservation (sans valeur de retour)
            await bookingRepository.UpdateBookingAsync(repositoryDto);

            // Récupérer la réservation mise à jour
            var updatedBooking = await bookingRepository.GetBookingByIdAsync(booking.Id);

            // Convertir le résultat en DTO pour le service
            return new BookingServiceDto(
                updatedBooking.Id,
                updatedBooking.RoomId,
                updatedBooking.PersonId,
                updatedBooking.BookingDate,
                updatedBooking.StartSlot,
                updatedBooking.EndSlot
            );
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("créneau horaire"))
        {
            // Si l'exception est due à un conflit de réservation, récupérer les créneaux disponibles
            var availableSlots = await GetAvailableSlotsForDay(booking.RoomId, booking.BookingDate);
            throw new BookingConflictException(
                "Ce créneau horaire est déjà réservé pour cette salle. Voici les créneaux disponibles pour cette journée.",
                availableSlots
            );
        }
    }
    private async Task ValidateBookingAsync(BookingServiceDto booking)
    {
        if (booking.BookingDate.Date < DateTime.UtcNow.Date)
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