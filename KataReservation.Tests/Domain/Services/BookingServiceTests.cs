using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Exceptions;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Services;
using Moq;

namespace KataReservation.Tests.Domain.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _service = new BookingService(_mockBookingRepository.Object);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCreateBooking_WhenDataIsValid()
    {
        // Arrange
        var bookingDto = new BookingServiceDto(
            Id: 0,
            RoomId: 1,
            PersonId: 1,
            BookingDate: DateTime.Today.AddDays(1),
            StartSlot: 9,
            EndSlot: 10
        );

        var expectedResult = new BookingRepositoryDto(
            Id: 1,
            RoomId: bookingDto.RoomId,
            PersonId: bookingDto.PersonId,
            BookingDate: bookingDto.BookingDate,
            StartSlot: bookingDto.StartSlot,
            EndSlot: bookingDto.EndSlot
        );

        _mockBookingRepository
            .Setup(r => r.GetConflictingBookingsAsync(
                bookingDto.RoomId,
                bookingDto.BookingDate,
                bookingDto.StartSlot,
                bookingDto.EndSlot))
            .ReturnsAsync(new List<BookingRepositoryDto>());

        _mockBookingRepository
            .Setup(r => r.CreateBookingAsync(It.IsAny<BookingRepositoryDto>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.CreateBookingAsync(bookingDto);

        // Assert
        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.RoomId, result.RoomId);
        Assert.Equal(expectedResult.PersonId, result.PersonId);
        Assert.Equal(expectedResult.BookingDate, result.BookingDate);
        Assert.Equal(expectedResult.StartSlot, result.StartSlot);
        Assert.Equal(expectedResult.EndSlot, result.EndSlot);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldThrowException_WhenDateIsInPast()
    {
        // Arrange
        var bookingDto = new BookingServiceDto(
            Id: 0,
            RoomId: 1,
            PersonId: 1,
            BookingDate: DateTime.Today.AddDays(-1),
            StartSlot: 9,
            EndSlot: 10
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateBookingAsync(bookingDto));
        Assert.Contains("date future", exception.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldThrowException_WhenStartSlotIsAfterEndSlot()
    {
        // Arrange
        var bookingDto = new BookingServiceDto(
            Id: 0,
            RoomId: 1,
            PersonId: 1,
            BookingDate: DateTime.Today.AddDays(1),
            StartSlot: 10,
            EndSlot: 9
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateBookingAsync(bookingDto));
        Assert.Contains("heure de début", exception.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldThrowException_WhenSlotsAreInvalid()
    {
        // Arrange
        var bookingDto = new BookingServiceDto(
            Id: 0,
            RoomId: 1,
            PersonId: 1,
            BookingDate: DateTime.Today.AddDays(1),
            StartSlot: 0,
            EndSlot: 25
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateBookingAsync(bookingDto));
        Assert.Contains("créneaux horaires", exception.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldThrowBookingConflictException_WhenBookingConflicts()
    {
        // Arrange
        var bookingDto = new BookingServiceDto(
            Id: 0,
            RoomId: 1,
            PersonId: 1,
            BookingDate: DateTime.Today.AddDays(1),
            StartSlot: 9,
            EndSlot: 10
        );

        var conflictingBookings = new List<BookingRepositoryDto>
        {
            new BookingRepositoryDto(
                Id: 1,
                RoomId: 1,
                PersonId: 2,
                BookingDate: bookingDto.BookingDate,
                StartSlot: 9,
                EndSlot: 10
            )
        };

        _mockBookingRepository
            .Setup(r => r.GetConflictingBookingsAsync(
                bookingDto.RoomId,
                bookingDto.BookingDate,
                bookingDto.StartSlot,
                bookingDto.EndSlot))
            .ReturnsAsync(conflictingBookings);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookingConflictException>(
            () => _service.CreateBookingAsync(bookingDto));
        Assert.Contains("créneau horaire", exception.Message);
    }

    [Fact]
    public async Task DeleteBookingAsync_ShouldReturnTrue_WhenBookingExists()
    {
        // Arrange
        var bookingId = 1;
        _mockBookingRepository
            .Setup(r => r.DeleteBookingAsync(bookingId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteBookingAsync(bookingId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteBookingAsync_ShouldReturnFalse_WhenBookingDoesNotExist()
    {
        // Arrange
        var bookingId = 1;
        _mockBookingRepository
            .Setup(r => r.DeleteBookingAsync(bookingId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteBookingAsync(bookingId);

        // Assert
        Assert.False(result);
    }
}