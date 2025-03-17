using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Interfaces.Services;

public interface IBookingService
{
    Task<BookingServiceDto> GetBookingByIdAsync(int id);
    Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto booking);
    Task<bool> DeleteBookingAsync(int id);


    //Task<BookingServiceDto> CreateBookingAsync(CreateBookingServiceDto createBookingDto);

    //Task<IEnumerable<BookingServiceDto>> GetBookingsAsync();
}