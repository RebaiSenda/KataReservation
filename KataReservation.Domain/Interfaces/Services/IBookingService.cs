using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Interfaces.Services;

public interface IBookingService
{
    //Task<IEnumerable<BookingServiceDto>> GetBookingsAsync();
    //Task<BookingServiceDto?> GetBookingByIdAsync(int id);
    //Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto booking);
    //Task<bool> DeleteBookingAsync(int id);
    Task<IEnumerable<RoomRepositoryDto>> GetAllRoomsAsync();
    Task<bool> CreateBookingAsync(BookingServiceDto booking);
    Task DeleteBookingAsync(int id);
    Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlotsAsync(int roomId, DateTime date);
}