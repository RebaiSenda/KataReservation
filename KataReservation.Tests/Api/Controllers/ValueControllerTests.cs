using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NFluent;
using NSubstitute;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Api.Controllers;
using KataReservation.Api.Dtos.Requests;

namespace KataReservation.Tests.Api.Controllers;

public class ValueControllerTests
{
    private readonly BookingController _valueController;
    private readonly IBookingService _bookingService = Substitute.For<IBookingService>();
    private readonly ILogger<BookingController> _logger = Substitute.For<ILogger<BookingController>>();

    public ValueControllerTests() =>
        _valueController = new BookingController(_bookingService, _logger);

    [Theory, AutoData]
    public async Task Should_Get_Values(IEnumerable<BookingServiceDto> values)
    {
        _bookingService.GetValuesAsync().Returns(values);

        var response = await _valueController.GetBookingsAsync();

        Check.That(response.Result).IsInstanceOf<OkObjectResult>();
    }

    [Theory, AutoData]
    public async Task Should_Get_Value_When_Value_Exists(int id, BookingServiceDto value)
    {
        _bookingService.GetValueByIdAsync(id).Returns(value);

        var response = await _valueController.GetBookingByIdAsync(id);

        Check.That(response.Result).IsInstanceOf<OkObjectResult>();
    }

    [Theory, AutoData]
    public async Task Should_Not_Get_Value_And_Return_404_When_Value_Doesnt_Exist(int id)
    {
        var response = await _valueController.GetBookingByIdAsync(id);

        Check.That(response.Result).IsInstanceOf<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task Should_Create_Value(CreateBookingRequest request, BookingServiceDto value)
    {
        _bookingService.CreateValueAsync(default!).ReturnsForAnyArgs(value);

        var response = await _valueController.CreateBookingAsync(request);

        Check.That(response.Result).IsInstanceOf<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Should_Not_Create_Value_And_Return_400_When_Bad_Request()
    {
        var request = new CreateBookingRequest(1, 1, 1, DateTime.Now, 1,1);
        _valueController.ModelState.AddModelError(string.Empty, string.Empty);

        var response = await _valueController.CreateBookingAsync(request);

        Check.That(response.Result).IsInstanceOf<BadRequestResult>();
    }

    [Theory, AutoData]
    public async Task Should_Update_Value(int id, UpdateBookingRequest request, BookingServiceDto value)
    {
        _bookingService.UpdateValueAsync(default!).ReturnsForAnyArgs(value);

        var response = await _valueController.UpdateValueAsync(id, request);

        Check.That(response.Result).IsInstanceOf<OkObjectResult>();
    }

    [Theory, AutoData]
    public async Task Should_Not_Update_Value_And_Return_400_When_Bad_Request(int id)
    {
        var request = new UpdateBookingRequest(1,1,DateTime.Now,1,1);
        _valueController.ModelState.AddModelError(string.Empty, string.Empty);

        var response = await _valueController.UpdateValueAsync(id, request);

        Check.That(response.Result).IsInstanceOf<BadRequestResult>();
    }

    [Theory, AutoData]
    public async Task Should_Not_Update_Value_And_Return_404_When_Value_Doesnt_Exist(int id, UpdateBookingRequest request)
    {
        var response = await _valueController.UpdateValueAsync(id, request);

        Check.That(response.Result).IsInstanceOf<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task Should_Delete_Value_When_Value_Exists(int id)
    {
        _bookingService.DeleteValueAsync(id).Returns(true);

        var response = await _valueController.DeleteValueAsync(id);

        Check.That(response).IsInstanceOf<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task Should_Not_Deletet_Value_And_Return_404_When_Value_Doesnt_Exist(int id)
    {
        var response = await _valueController.DeleteValueAsync(id);

        Check.That(response).IsInstanceOf<NotFoundResult>();
    }
}