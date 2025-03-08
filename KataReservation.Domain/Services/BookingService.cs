using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Models;

namespace KataReservation.Domain.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    public async Task<IEnumerable<BookingServiceDto>> GetBookingsAsync()
    {
        var BookingsDtos = await bookingRepository.GetAllBookingsAsync();
        var Bookings = BookingsDtos.Select(v => new Booking(v.Id, v.RoomId, v.PersonId, v.BookingDate, v.StartSlot, v.EndSlot));
        return Bookings.Select(v => new BookingServiceDto(v));
    }

    public async Task<BookingServiceDto?> GetBookingByIdAsync(int id)
    {
        var BookingDto = await bookingRepository.GetBookingByIdAsync(id);

        if (BookingDto is null)
        {
            return null;
        }
        
        var Booking = new Booking(BookingDto.Id, BookingDto.RoomId, BookingDto.PersonId, BookingDto.BookingDate, BookingDto.StartSlot, BookingDto.EndSlot);
        return new BookingServiceDto(Booking);
    }

    //public async Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto BookingServiceDto)
    //{
    //    var BookingRepositoryDto = new BookingRepositoryDto(BookingServiceDto);
    //    var BookingDto = await bookingRepository.CreateBookingAsync(BookingRepositoryDto);
    //    var Booking = new Booking(BookingDto.Id, BookingDto.RoomId, BookingDto.PersonId, BookingDto.BookingDate, BookingDto.StartSlot, BookingDto.EndSlot);
    //    return new BookingServiceDto(Booking);
    //}

    public async Task<bool> DeleteBookingAsync(int id) =>
        await bookingRepository.DeleteBookingAsync(id);

    public Task<IEnumerable<RoomRepositoryDto>> GetAllRoomsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateBookingAsync(BookingServiceDto booking)
    {
        throw new NotImplementedException();
    }

    Task IBookingService.DeleteBookingAsync(int id)
    {
        return DeleteBookingAsync(id);
    }

    public Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlotsAsync(int roomId, DateTime date)
    {
        throw new NotImplementedException();
    }
}