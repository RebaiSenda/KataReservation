using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Models;

namespace KataReservation.Domain.Services;

public class BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IRoomRepository _roomRepository = roomRepository;
    public async Task<IEnumerable<BookingServiceDto>> GetBookingsAsync()
    {
        var BookingsDtos = await bookingRepository.GetAllBookingsAsync();
        var Bookings = BookingsDtos.Select(v => new Booking(v.Id, v.RoomId, v.PersonId, v.BookingDate, v.StartSlot, v.EndSlot));
        return Bookings.Select(v => new BookingServiceDto(v));
    }

    public async Task<BookingServiceDto?> GetBookingByIdAsync(int id)
    {
        var BookingDto = await bookingRepository.GetBookingByIdAsync(id);

        if (BookingDto is null)
        {
            return null;
        }
        
        var Booking = new Booking(BookingDto.Id, BookingDto.RoomId, BookingDto.PersonId, BookingDto.BookingDate, BookingDto.StartSlot, BookingDto.EndSlot);
        return new BookingServiceDto(Booking);
    }
    public async Task<bool> DeleteBookingAsync(int id) =>
        await bookingRepository.DeleteBookingAsync(id);

    public Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlotsAsync(int roomId, DateTime date)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<BookingServiceDto>> GetBookingsByDateAsync(DateTime date)
    {
        var bookingsDtos = await bookingRepository.GetBookingsByDateAsync(date);
        var bookings = bookingsDtos.Select(v => new Booking(v.Id, v.RoomId, v.PersonId, v.BookingDate, v.StartSlot, v.EndSlot));
        return bookings.Select(v => new BookingServiceDto(v));
    }

    public async Task<IEnumerable<BookingServiceDto>> GetBookingsByRoomAndDateAsync(int roomId, DateTime date)
    {
        var bookingsDtos = await bookingRepository.GetBookingsByRoomAndDateAsync(roomId, date);
        var bookings = bookingsDtos.Select(v => new Booking(v.Id, v.RoomId, v.PersonId, v.BookingDate, v.StartSlot, v.EndSlot));
        return bookings.Select(v => new BookingServiceDto(v));
    }

    public async Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto bookingDto)
    {
        var bookingRepositoryDto = new BookingRepositoryDto(
            bookingDto.Id,
            bookingDto.RoomId,
            bookingDto.PersonId,
            bookingDto.BookingDate,
            bookingDto.StartSlot,
            bookingDto.EndSlot
        );

        var createdBookingDto = await bookingRepository.AddBookingAsync(bookingRepositoryDto);

        var booking = new Booking(
            createdBookingDto.Id,
            createdBookingDto.RoomId,
            createdBookingDto.PersonId,
            createdBookingDto.BookingDate,
            createdBookingDto.StartSlot,
            createdBookingDto.EndSlot
        );

        return new BookingServiceDto(booking);
    }

    public async Task<BookingServiceDto> UpdateBookingAsync(BookingServiceDto bookingDto)
    {
        var existingBooking = await GetBookingByIdAsync(bookingDto.Id);
        if (existingBooking is null)
        {
            throw new KeyNotFoundException($"Booking with ID {bookingDto.Id} not found");
        }

        var bookingRepositoryDto = new BookingRepositoryDto(
            bookingDto.Id,
            bookingDto.RoomId,
            bookingDto.PersonId,
            bookingDto.BookingDate,
            bookingDto.StartSlot,
            bookingDto.EndSlot
        );
        return bookingDto;
    }
    public async Task<bool> IsSlotAvailableAsync(int roomId, DateTime date, int startSlot, int endSlot)
    {
        // Vérifier si la salle existe
        var room = await _roomRepository.GetRoomByIdAsync(roomId);
        if (room == null)
        {
            return false;
        }

        // Récupérer toutes les réservations pour cette salle et cette date
        var bookings = await _bookingRepository.GetByRoomAndDateAsync(roomId, date);

        // Vérifier s'il y a des conflits
        foreach (var booking in bookings)
        {
            // Vérifier si les créneaux se chevauchent
            if (!(endSlot <= booking.StartSlot || startSlot >= booking.EndSlot))
            {
                return false;
            }
        }

        return true;
    }
    public async Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int roomId, DateTime date)
    {
        // Vérifier si la salle existe
        var room = await _roomRepository.GetRoomByIdAsync(roomId);
        if (room == null)
        {
            return new List<AvailableSlotDto>();
        }

        // Récupérer toutes les réservations pour cette salle et cette date
        var bookings = await _bookingRepository.GetByRoomAndDateAsync(roomId, date);

        // Créer une liste de tous les créneaux possibles (par exemple, de 8h à 18h)
        var allSlots = new List<AvailableSlotDto>();
        for (int i = 8; i < 18; i++)
        {
            allSlots.Add(new AvailableSlotDto(i, i + 1));
        }

        // Supprimer les créneaux déjà réservés
        foreach (var booking in bookings)
        {
            allSlots.RemoveAll(slot =>
                !(slot.EndSlot <= booking.StartSlot || slot.StartSlot >= booking.EndSlot));
        }

        // Fusionner les créneaux adjacents
        var mergedSlots = new List<AvailableSlotDto>();
        if (allSlots.Count > 0)
        {
            var currentSlot = allSlots[0];

            for (int i = 1; i < allSlots.Count; i++)
            {
                if (allSlots[i].StartSlot == currentSlot.EndSlot)
                {
                    // Fusionner les créneaux
                    currentSlot = new AvailableSlotDto(currentSlot.StartSlot, allSlots[i].EndSlot);
                }
                else
                {
                    // Ajouter le créneau actuel et passer au suivant
                    mergedSlots.Add(currentSlot);
                    currentSlot = allSlots[i];
                }
            }

            // Ajouter le dernier créneau
            mergedSlots.Add(currentSlot);
        }

        return mergedSlots;
    }
}