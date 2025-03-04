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
    private readonly BookingController _BookingController;
    private readonly IBookingService _bookingService = Substitute.For<IBookingService>();
    private readonly ILogger<BookingController> _logger = Substitute.For<ILogger<BookingController>>();

    public BookingControllerTests() =>
        _BookingController = new BookingController(_bookingService, _logger);

    [Theory, AutoData]
    public async Task Should_Get_Bookings(IEnumerable<BookingServiceDto> Bookings)
    {
        _bookingService.GetBookingsAsync().Returns(Bookings);

        var response = await _BookingController.GetBookingsAsync();

        Check.That(response.Result).IsInstanceOf<OkObjectResult>();
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
        var request = new CreateBookingRequest(1, 1, 1, DateTime.Now, 1,1);
        _BookingController.ModelState.AddModelError(string.Empty, string.Empty);

        var response = await _BookingController.CreateBookingAsync(request);

        Check.That(response.Result).IsInstanceOf<BadRequestResult>();
    }

    [Theory, AutoData]
    public async Task Should_Update_Booking(int id, UpdateBookingRequest request, BookingServiceDto Booking)
    {
        _bookingService.UpdateBookingAsync(default!).ReturnsForAnyArgs(Booking);

        var response = await _BookingController.UpdateBookingAsync(id, request);

        Check.That(response.Result).IsInstanceOf<OkObjectResult>();
    }

    [Theory, AutoData]
    public async Task Should_Not_Update_Booking_And_Return_400_When_Bad_Request(int id)
    {
        var request = new UpdateBookingRequest(1,1,DateTime.Now,1,1);
        _BookingController.ModelState.AddModelError(string.Empty, string.Empty);

        var response = await _BookingController.UpdateBookingAsync(id, request);

        Check.That(response.Result).IsInstanceOf<BadRequestResult>();
    }

    [Theory, AutoData]
    public async Task Should_Not_Update_Booking_And_Return_404_When_Booking_Doesnt_Exist(int id, UpdateBookingRequest request)
    {
        var response = await _BookingController.UpdateBookingAsync(id, request);

        Check.That(response.Result).IsInstanceOf<NotFoundResult>();
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