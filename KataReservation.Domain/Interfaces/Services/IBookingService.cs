using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Interfaces.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingServiceDto>> GetBookingsAsync();
    Task<BookingServiceDto> GetBookingByIdAsync(int id);
    Task<IEnumerable<BookingServiceDto>> GetBookingsByDateAsync(DateTime date);
    Task<IEnumerable<BookingServiceDto>> GetBookingsByRoomAndDateAsync(int roomId, DateTime date);
    Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto booking);
    Task<BookingServiceDto> UpdateBookingAsync(BookingServiceDto booking);
    Task<bool> DeleteBookingAsync(int id);
    Task<bool> IsSlotAvailableAsync(int roomId, DateTime date, int startSlot, int endSlot);

    /// Récupère tous les créneaux disponibles pour une salle et une date spécifiques
    Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int roomId, DateTime date);
}