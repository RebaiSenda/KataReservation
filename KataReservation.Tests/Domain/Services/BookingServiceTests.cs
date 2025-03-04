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
        _bookingRepository.GetValuesAsync().Returns(values);

        var result = await _bookingService.GetValuesAsync();

        Check.That(result).IsNotNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Get_Value_When_Value_Exists(int id, BookingRepositoryDto value)
    {
        _bookingRepository.GetValueByIdAsync(id).Returns(value);

        var result = await _bookingService.GetValueByIdAsync(id);

        Check.That(result).IsNotNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Not_Get_Value_And_Return_Null_When_Value_Doesnt_Exist(int id)
    {
        _bookingRepository.GetValueByIdAsync(id).Returns((BookingRepositoryDto?)null);

        var result = await _bookingService.GetValueByIdAsync(id);

        Check.That(result).IsNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Create_Value(BookingServiceDto valueServiceDto, BookingRepositoryDto valueRepositoryDto)
    {
        _bookingRepository.CreateValueAsync(default!).ReturnsForAnyArgs(valueRepositoryDto);

        var result = await _bookingService.CreateValueAsync(valueServiceDto);

        Check.That(result).IsNotNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Update_Value_When_Value_Exists(BookingServiceDto valueServiceDto, BookingRepositoryDto valueRepositoryDto)
    {
        _bookingRepository.UpdateValueAsync(default!).ReturnsForAnyArgs(valueRepositoryDto);

        var result = await _bookingService.UpdateValueAsync(valueServiceDto);

        Check.That(result).IsNotNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Not_Update_Value_And_Return_Value_When_Value_Doesnt_Exist(BookingServiceDto value)
    {
        _bookingRepository.UpdateValueAsync(default!).ReturnsForAnyArgs((BookingRepositoryDto?)null);

        var result = await _bookingService.UpdateValueAsync(value);

        Check.That(result).IsNull();
    }

    [Theory, AutoData]
    public async Task Shoud_Delete_Value(int id)
    {
        _bookingRepository.DeleteValueAsync(id).Returns(true);

        var result = await _bookingService.DeleteValueAsync(id);

        Check.That(result).IsTrue();
    }
}