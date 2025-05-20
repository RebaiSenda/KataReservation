using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Interfaces.Services;

public interface IBookingService
{

    Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto booking);
    Task<bool> DeleteBookingAsync(int bookingId);
    Task<BookingServiceDto?> GetBookingAsync(int bookingId);
    Task<BookingServiceDto?> UpdateBookingAsync(BookingServiceDto booking);
    Task<List<SlotDto>> GetAvailableSlotsForDay(int roomId, DateTime date);

}