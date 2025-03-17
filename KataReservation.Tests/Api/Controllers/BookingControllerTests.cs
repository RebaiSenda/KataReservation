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

public class BookingControllerTests
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;
    private readonly BookingController _BookingController;

    public BookingControllerTests()
    {
        _bookingService = Substitute.For<IBookingService>();
        _logger = Substitute.For<ILogger<BookingController>>();

        _BookingController = new BookingController(_bookingService, _logger);
    }

    [Theory, AutoData]
    public async Task Should_Create_Booking(CreateBookingRequest request, BookingServiceDto Booking)
    {
        _bookingService.CreateBookingAsync(default!).ReturnsForAnyArgs(Booking);
        var response = await _BookingController.CreateBookingAsync(request);
        Check.That(response.Result).IsInstanceOf<CreatedAtActionResult>();
    }

    [Theory, AutoData]
    public async Task Should_Delete_Booking_When_Booking_Exists(int id)
    {
        _bookingService.DeleteBookingAsync(id).Returns(true);
        var response = await _BookingController.DeleteBookingAsync(id);
        Check.That(response).IsInstanceOf<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task Should_Not_Deletet_Booking_And_Return_404_When_Booking_Doesnt_Exist(int id)
    {
        _bookingService.DeleteBookingAsync(id).Returns(false);

        var response = await _BookingController.DeleteBookingAsync(id);
        Check.That(response).IsInstanceOf<NotFoundResult>();
    }
}