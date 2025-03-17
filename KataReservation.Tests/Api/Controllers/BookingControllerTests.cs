using Microsoft.Extensions.Logging;
using NSubstitute;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Api.Controllers;

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

}