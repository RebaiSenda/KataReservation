using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Models;
using KataReservation.Domain.Services;
using NSubstitute;

namespace KataReservation.Tests.Domain.Services;

public class BookingServiceTests
{
    //private readonly BookingService _bookingService;
    //private readonly IBookingRepository _bookingRepository = Substitute.For<IBookingRepository>();

    //public BookingServiceTests() =>
    //    _bookingService = new BookingService(_bookingRepository);

    //[Fact]
    //public async Task GetBookingsAsync_ReturnsAllBookings()
    //{
    //    var expectedBookings = new List<Booking>
    //        {
    //            new Booking { Id = 1, RoomId = 1, Title = "Meeting 1" },
    //            new Booking { Id = 2, RoomId = 2, Title = "Meeting 2" }
    //        };

    //    _mockRepository.Setup(repo => repo.GetAllAsync())
    //        .ReturnsAsync(expectedBookings);

    //    var result = await _service.GetBookingsAsync();

    //    var bookings = result.ToList();
    //    Assert.Equal(2, bookings.Count);
    //    Assert.Equal(expectedBookings[0].Id, bookings[0].Id);
    //    Assert.Equal(expectedBookings[1].Id, bookings[1].Id);
    //    _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    //}

}