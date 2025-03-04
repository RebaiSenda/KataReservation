using KataReservation.Domain.Dtos.Repositories;

namespace KataReservation.Domain.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<BookingRepositoryDto>> GetBookingsAsync();
    Task<BookingRepositoryDto?> GetBookingByIdAsync(int id);
    Task<BookingRepositoryDto> CreateBookingAsync(BookingRepositoryDto booking);
    Task<BookingRepositoryDto?> UpdateBookingAsync(BookingRepositoryDto Booking);
    Task<bool> DeleteBookingAsync(int id);
}