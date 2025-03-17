using KataReservation.Domain.Dtos.Repositories;

namespace KataReservation.Domain.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<BookingRepositoryDto> AddBookingAsync(BookingRepositoryDto booking);
    Task<bool> DeleteBookingAsync(int id);
    Task<BookingRepositoryDto> GetBookingByIdAsync(int id);
    //Task<BookingRepositoryDto> CreateBookingAsync(int roomId, int personId, DateTime bookingDate, int startSlot, int endSlot);
    //Task<IEnumerable<BookingRepositoryDto>> GetBookingsAsync();
    //Task<IEnumerable<BookingRepositoryDto>> GetBookingsByRoomAndDateAsync(int roomId, DateTime date);
}