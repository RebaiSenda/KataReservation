using KataReservation.Domain.Dtos.Repositories;

namespace KataReservation.Domain.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<BookingRepositoryDto> CreateBookingAsync(BookingRepositoryDto booking);
    Task<bool> DeleteBookingAsync(int bookingId);

    Task<BookingRepositoryDto?> GetBookingByIdAsync(int bookingId);
    Task<IEnumerable<BookingRepositoryDto>> GetConflictingBookingsAsync(int roomId, DateTime bookingDate, int startSlot, int endSlot);  
    Task<IEnumerable<BookingRepositoryDto>> GetBookingsByRoomAndDateAsync(int roomId, DateTime date);
}