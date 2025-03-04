using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Interfaces.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingServiceDto>> GetBookingsAsync();
    Task<BookingServiceDto?> GetBookingByIdAsync(int id);
    Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto booking);
    Task<BookingServiceDto?> UpdateBookingAsync(BookingServiceDto booking);
    Task<bool> DeleteBookingAsync(int id);
}