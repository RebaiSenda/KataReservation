using AutoFixture.Xunit2;
using NFluent;
using NSubstitute;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Services;

namespace KataReservation.Tests.Domain.Services;

public class BookingServiceTests
{
    private readonly BookingService _bookingService;
    private readonly IBookingRepository _bookingRepository = Substitute.For<IBookingRepository>();

    public BookingServiceTests() =>
        _bookingService = new BookingService(_bookingRepository);

    [Theory, AutoData]
    public async Task Shoud_Get_Values(IEnumerable<BookingRepositoryDto> values)
    {
        _bookingRepository.GetBookingsAsync().Returns(values);

        var result = await _bookingService.GetBookingsAsync();

        Check.That(result).IsNotNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Get_Value_When_Value_Exists(int id, BookingRepositoryDto value)
    {
        _bookingRepository.GetBookingByIdAsync(id).Returns(value);

        var result = await _bookingService.GetBookingByIdAsync(id);

        Check.That(result).IsNotNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Not_Get_Value_And_Return_Null_When_Value_Doesnt_Exist(int id)
    {
        _bookingRepository.GetBookingByIdAsync(id).Returns((BookingRepositoryDto?)null);

        var result = await _bookingService.GetBookingByIdAsync(id);

        Check.That(result).IsNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Create_Value(BookingServiceDto valueServiceDto, BookingRepositoryDto valueRepositoryDto)
    {
        _bookingRepository.CreateBookingAsync(default!).ReturnsForAnyArgs(valueRepositoryDto);

        var result = await _bookingService.CreateBookingAsync(valueServiceDto);

        Check.That(result).IsNotNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Delete_Value(int id)
    {
        _bookingRepository.DeleteBookingAsync(id).Returns(true);

        var result = await _bookingService.DeleteBookingAsync(id);

        Check.That(result).IsTrue();
    }
}