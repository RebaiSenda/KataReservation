using KataReservation.Domain.Dtos.Repositories;

namespace KataReservation.Domain.Interfaces.Repositories;

public interface IBookingRepository
{
    //Task<IEnumerable<BookingRepositoryDto>> GetBookingsAsync();
    //Task<BookingRepositoryDto?> GetBookingByIdAsync(int id);
    //Task<BookingRepositoryDto> CreateBookingAsync(BookingRepositoryDto booking);
    Task<bool> DeleteBookingAsync(int id);
    Task<IEnumerable<BookingRepositoryDto>> GetAllBookingsAsync();
    Task<BookingRepositoryDto> GetBookingByIdAsync(int id);
    Task<IEnumerable<BookingRepositoryDto>> GetBookingsByDateAsync(DateTime date);
    Task<IEnumerable<BookingRepositoryDto>> GetBookingsByRoomAndDateAsync(int roomId, DateTime date);
    Task<BookingRepositoryDto> AddBookingAsync(BookingRepositoryDto booking);
    //Task DeleteBookingAsync(int id);
}