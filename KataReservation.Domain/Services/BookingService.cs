using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Models;

namespace KataReservation.Domain.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    public async Task<IEnumerable<BookingServiceDto>> GetValuesAsync()
    {
        var valuesDtos = await bookingRepository.GetValuesAsync();
        var values = valuesDtos.Select(v => new Booking(v.Id, v.RoomId, v.PersonId, v.BookingDate, v.StartSlot, v.EndSlot));
        return values.Select(v => new BookingServiceDto(v));
    }

    public async Task<BookingServiceDto?> GetValueByIdAsync(int id)
    {
        var valueDto = await bookingRepository.GetValueByIdAsync(id);

        if (valueDto is null)
        {
            return null;
        }
        
        var value = new Booking(valueDto.Id, valueDto.RoomId, valueDto.PersonId, valueDto.BookingDate, valueDto.StartSlot, valueDto.EndSlot);
        return new BookingServiceDto(value);
    }

    public async Task<BookingServiceDto> CreateValueAsync(BookingServiceDto valueServiceDto)
    {
        var valueRepositoryDto = new BookingRepositoryDto(valueServiceDto);
        var valueDto = await bookingRepository.CreateValueAsync(valueRepositoryDto);
        var value = new Booking(valueDto.Id, valueDto.RoomId, valueDto.PersonId, valueDto.BookingDate, valueDto.StartSlot, valueDto.EndSlot);
        return new BookingServiceDto(value);
    }

    public async Task<BookingServiceDto?> UpdateValueAsync(BookingServiceDto valueServiceDto)
    {
        var valueRepositoryDto = new BookingRepositoryDto(valueServiceDto);
        var valueDto = await bookingRepository.UpdateValueAsync(valueRepositoryDto);
        
        if (valueDto is null)
        {
            return null;
        }

        var value = new Booking(valueDto.Id, valueDto.RoomId, valueDto.PersonId, valueDto.BookingDate, valueDto.StartSlot, valueDto.EndSlot);
        return new BookingServiceDto(value);
    }

    public async Task<bool> DeleteValueAsync(int id) =>
        await bookingRepository.DeleteValueAsync(id);
}