using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NFluent;
using NSubstitute;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Api.Controllers;
using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Models;
using Moq;
using KataReservation.Domain.Dtos.Repositories;


namespace KataReservation.Tests.Api.Controllers;

public class BookingControllerTests
{
    private readonly BookingController _BookingController;
    private readonly IBookingService _bookingService = Substitute.For<IBookingService>();
    private readonly ILogger<BookingController> _logger = Substitute.For<ILogger<BookingController>>();
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<ILogger<BookingController>> _mockLogger;
    private readonly BookingController _controller;
    //public BookingControllerTests() =>
    //    _BookingController = new BookingController(_bookingService, _logger);
    public BookingControllerTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _mockLogger = new Mock<ILogger<BookingController>>();
        _controller = new BookingController(_mockBookingService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetBookingsByRoomAndDateAsync_ReturnsOkResult_WithBookingsResponse()
    {
        int roomId = 1;
        DateTime date = new DateTime(2024, 3, 11);

        var bookings = new List<BookingServiceDto>
        {
            new BookingServiceDto(1, roomId, 101, date, 9, 10),
            new BookingServiceDto(2, roomId, 102, date, 14, 16)
        };

        _mockBookingService
            .Setup(s => s.GetBookingsByRoomAndDateAsync(roomId, date))
            .ReturnsAsync(bookings);

        var result = await _controller.GetBookingsByRoomAndDateAsync(roomId, date);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<BookingsResponse>(okResult.Value);
        Assert.Equal(bookings.Count, response.Values.Count());
    }
    [Theory, AutoData]
    public async Task Should_Get_Booking_When_Booking_Exists(int id, BookingServiceDto Booking)
    {
        _bookingService.GetBookingByIdAsync(id).Returns(Booking);

        var response = await _BookingController.GetBookingByIdAsync(id);

        Check.That(response.Result).IsInstanceOf<OkObjectResult>();
    }

    [Theory, AutoData]
    public async Task Should_Not_Get_Booking_And_Return_404_When_Booking_Doesnt_Exist(int id)
    {
        var response = await _BookingController.GetBookingByIdAsync(id);

        Check.That(response.Result).IsInstanceOf<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task Should_Create_Booking(CreateBookingRequest request, BookingServiceDto Booking)
    {
        _bookingService.CreateBookingAsync(default!).ReturnsForAnyArgs(Booking);

        var response = await _BookingController.CreateBookingAsync(request);

        Check.That(response.Result).IsInstanceOf<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Should_Not_Create_Booking_And_Return_400_When_Bad_Request()
    {
        var request = new CreateBookingRequest(1, 1, 1, DateTime.Now, 1, 1);
        _BookingController.ModelState.AddModelError(string.Empty, string.Empty);

        var response = await _BookingController.CreateBookingAsync(request);

        Check.That(response.Result).IsInstanceOf<BadRequestResult>();
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
        var response = await _BookingController.DeleteBookingAsync(id);

        Check.That(response).IsInstanceOf<NotFoundResult>();
    }
}
