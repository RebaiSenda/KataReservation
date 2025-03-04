using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Interfaces.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingServiceDto>> GetValuesAsync();
    Task<BookingServiceDto?> GetValueByIdAsync(int id);
    Task<BookingServiceDto> CreateValueAsync(BookingServiceDto value);
    Task<BookingServiceDto?> UpdateValueAsync(BookingServiceDto value);
    Task<bool> DeleteValueAsync(int id);
}