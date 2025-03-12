using KataReservation.Domain.Dtos.Repositories;

namespace KataReservation.Domain.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<BookingRepositoryDto>> GetBookingsAsync();
    Task<bool> DeleteBookingAsync(int id);
    Task<IEnumerable<BookingRepositoryDto>> GetAllBookingsAsync();
    Task<BookingRepositoryDto> GetBookingByIdAsync(int id);
    Task<IEnumerable<BookingRepositoryDto>> GetBookingsByDateAsync(DateTime date);
    Task<IEnumerable<BookingRepositoryDto>> GetBookingsByRoomAndDateAsync(int roomId, DateTime date);
    Task<BookingRepositoryDto> AddBookingAsync(BookingRepositoryDto booking);
    Task<List<BookingRepositoryDto>> GetByRoomAndDateAsync(int roomId, DateTime date);
    //void GetAllAsync();
}