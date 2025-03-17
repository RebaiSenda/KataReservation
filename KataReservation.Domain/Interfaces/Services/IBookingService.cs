using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Interfaces.Services;

public interface IBookingService
{

    Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto booking);
    Task<bool> DeleteBookingAsync(int bookingId);

}