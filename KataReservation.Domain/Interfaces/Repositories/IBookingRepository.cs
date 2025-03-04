using KataReservation.Domain.Dtos.Repositories;

namespace KataReservation.Domain.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<BookingRepositoryDto>> GetValuesAsync();
    Task<BookingRepositoryDto?> GetValueByIdAsync(int id);
    Task<BookingRepositoryDto> CreateValueAsync(BookingRepositoryDto value);
    Task<BookingRepositoryDto?> UpdateValueAsync(BookingRepositoryDto value);
    Task<bool> DeleteValueAsync(int id);
}