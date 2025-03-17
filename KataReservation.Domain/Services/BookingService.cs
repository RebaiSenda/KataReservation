using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Models;

namespace KataReservation.Domain.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    //private readonly IBookingRepository _bookingRepository = bookingRepository;
    //private readonly IRoomRepository _roomRepository = roomRepository;



    public async Task<BookingServiceDto> GetBookingByIdAsync(int id)
    {
        var BookingDto = await bookingRepository.GetBookingByIdAsync(id);

        if (BookingDto is null)
        {
            return null!;
        }

        var Booking = new Booking(BookingDto.Id, BookingDto.RoomId, BookingDto.PersonId, BookingDto.BookingDate, BookingDto.StartSlot, BookingDto.EndSlot);
        return new BookingServiceDto(Booking);
    }
    public async Task<bool> DeleteBookingAsync(int id) =>
        await bookingRepository.DeleteBookingAsync(id);


    public async Task<BookingServiceDto> CreateBookingAsync(BookingServiceDto bookingDto)
    {
        var bookingRepositoryDto = new BookingRepositoryDto(
            bookingDto.Id,
            bookingDto.RoomId,
            bookingDto.PersonId,
            bookingDto.BookingDate,
            bookingDto.StartSlot,
            bookingDto.EndSlot
        );

        var createdBookingDto = await bookingRepository.AddBookingAsync(bookingRepositoryDto);

        var booking = new Booking(
            createdBookingDto.Id,
            createdBookingDto.RoomId,
            createdBookingDto.PersonId,
            createdBookingDto.BookingDate,
            createdBookingDto.StartSlot,
            createdBookingDto.EndSlot
        );

        return new BookingServiceDto(booking);
    }
}