using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NFluent;
using KataReservation.Api.Controllers;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using Moq;
using ApiRoomResponse = KataReservation.Api.Dtos.Responses.RoomResponse;
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
        var rooms = new List<KataReservation.Domain.Dtos.Services.RoomServiceDto>
    {
        new KataReservation.Domain.Dtos.Services.RoomServiceDto(
            new KataReservation.Domain.Models.Room(1, "Room 1")),
        new KataReservation.Domain.Dtos.Services.RoomServiceDto(
            new KataReservation.Domain.Models.Room(2, "Room 2"))
    };
        _mockRoomService.Setup(s => s.GetRoomsAsync()).ReturnsAsync(rooms);

        var result = await _controller.GetRoomsAsync();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var roomsResponse = Assert.IsType<KataReservation.Api.Dtos.Responses.RoomsResponse>(okResult.Value);
        Assert.Equal(2, roomsResponse.Values.Count());
    }
    [Fact]
    public async Task GetRoomByIdAsync_WithExistingId_ReturnsOkResult()
    {
        var room = new KataReservation.Domain.Dtos.Services.RoomServiceDto(new KataReservation.Domain.Models.Room(1, "Room 1"));
        _mockRoomService.Setup(s => s.GetRoomByIdAsync(1)).ReturnsAsync(room);

        var result = await _controller.GetRoomByIdAsync(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(room, okResult.Value);
    }
    [Fact]
    public async Task GetRoomByIdAsync_WithNonExistingId_ReturnsNotFound()
    {
        _mockRoomService.Setup(s => s.GetRoomByIdAsync(999)).ReturnsAsync((KataReservation.Domain.Dtos.Services.RoomServiceDto)null);

        var result = await _controller.GetRoomByIdAsync(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }
    [Fact]
    public async Task CreateRoomAsync_WithValidRequest_ReturnsCreatedAtAction()
    {
        var request = new CreateRoomRequest("New Room");
        var createdRoom = new KataReservation.Domain.Dtos.Services.RoomServiceDto(new KataReservation.Domain.Models.Room(1, "New Room"));
        _mockRoomService.Setup(s => s.CreateRoomAsync(request.RoomName)).ReturnsAsync(createdRoom);
        var result = await _controller.CreateRoomAsync(request);
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(RoomController.GetRoomByIdAsync), createdAtResult.ActionName);
        Assert.Equal(1, createdAtResult.RouteValues["id"]);
        //var roomResponse = Assert.IsType<RoomResponse>(createdAtResult.Value);
        var roomResponse = Assert.IsType<ApiRoomResponse>(createdAtResult.Value);
        Assert.Equal(createdRoom.Id, roomResponse.Id);
        Assert.Equal(createdRoom.RoomName, roomResponse.RoomName);
    }
    [Fact]
        public async Task UpdateRoomAsync_WithExistingId_ReturnsOkResult()
    {
        var request = new UpdateRoomRequest ("Updated Room") ;
        var updatedRoom = new KataReservation.Domain.Dtos.Services.RoomServiceDto(new KataReservation.Domain.Models.Room(1, "Updated Room"));
        _mockRoomService.Setup(s => s.UpdateRoomAsync(1, request.RoomName)).ReturnsAsync(updatedRoom);

        var result = await _controller.UpdateRoomAsync(1, request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var roomResponse = Assert.IsType<RoomResponse>(okResult.Value);
        Assert.Equal(updatedRoom.Id, roomResponse.Room.Id);
        Assert.Equal(updatedRoom.RoomName, roomResponse.Room.RoomName);
    }
    [Fact]
    public async Task UpdateRoomAsync_WithNonExistingId_ReturnsNotFound()
    {
        var request = new UpdateRoomRequest ("Updated Room" );
        //_mockRoomService.Setup(s => s.UpdateRoomAsync(999, request.RoomName)).ReturnsAsync((RoomServiceDto)null);
        _mockRoomService.Setup(s => s.UpdateRoomAsync(999, request.RoomName))
    .ReturnsAsync((KataReservation.Domain.Dtos.Services.RoomServiceDto)null);
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
