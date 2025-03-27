using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NFluent;
using KataReservation.Api.Controllers;
using KataReservation.Domain.Interfaces.Services;
using Moq;
using RoomDto = KataReservation.Domain.Dtos.Services.RoomServiceDto;
using Room = KataReservation.Domain.Models.Room;
using ApiRoomsResponse = KataReservation.Api.Dtos.Responses.RoomsResponse;
using KataReservation.Api.Dtos.Requests;
namespace KataReservation.Tests.Api.Controllers;

public class RoomControllerTests
{

    private readonly Mock<IRoomService> _mockRoomService;
    private readonly Mock<ILogger<RoomController>> _mockLogger;
    private readonly RoomController _controller;
    public RoomControllerTests()
    {
        _mockRoomService = new Mock<IRoomService>();
        _mockLogger = new Mock<ILogger<RoomController>>();
        _controller = new RoomController(_mockRoomService.Object, _mockLogger.Object);
    }
    [Fact]
    public async Task GetRoomsAsync_ReturnsOkResult_WithRoomsResponse()
    {
        var rooms = new List<RoomDto>
    {
        new RoomDto(
            new Room(1, "Room 1")),
        new RoomDto(
            new Room(2, "Room 2"))
    };
        _mockRoomService.Setup(s => s.GetRoomsAsync()).ReturnsAsync(rooms);

        var result = await _controller.GetRoomsAsync();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var roomsResponse = Assert.IsType<ApiRoomsResponse>(okResult.Value);
        Assert.Equal(2, roomsResponse.Rooms.Count());
    }
    [Fact]
    public async Task GetRoomByIdAsync_WithExistingId_ReturnsOkResult()
    {
        var room = new RoomDto(new Room(1, "Room 1"));
        _mockRoomService.Setup(s => s.GetRoomByIdAsync(1)).ReturnsAsync(room);

        var result = await _controller.GetRoomByIdAsync(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(room, okResult.Value);
    }
    [Fact]
    public async Task GetRoomByIdAsync_WithNonExistingId_ReturnsNotFound()
    {
        _mockRoomService.Setup(s => s.GetRoomByIdAsync(999)).ReturnsAsync((RoomDto?)null);

        var result = await _controller.GetRoomByIdAsync(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }
    [Fact]
    public async Task UpdateRoomAsync_WithNonExistingId_ReturnsNotFound()
    {
        var request = new UpdateRoomRequest ("Updated Room" );
        _mockRoomService.Setup(s => s.UpdateRoomAsync(999, request.RoomName))
    .ReturnsAsync((RoomDto?)null);
        var result = await _controller.UpdateRoomAsync(999, request);

        Assert.IsType<NotFoundResult>(result.Result);
    }
    [Fact]
    public async Task DeleteRoomAsync_WithExistingId_ReturnsNoContent()
    {
        _mockRoomService.Setup(s => s.DeleteRoomAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteRoomAsync(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteRoomAsync_WithNonExistingId_ReturnsNotFound()
    {
        _mockRoomService.Setup(s => s.DeleteRoomAsync(999)).ReturnsAsync(false);

        var result = await _controller.DeleteRoomAsync(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
