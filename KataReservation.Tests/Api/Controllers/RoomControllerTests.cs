using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NFluent;
using NSubstitute;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Api.Controllers;

namespace KataReservation.Tests.Api.Controllers;

public class RoomControllerTests
{
    private readonly RoomController _roomController;
    private readonly IRoomService _roomService = Substitute.For<IRoomService>();
    private readonly ILogger<RoomController> _logger = Substitute.For<ILogger<RoomController>>();

    public RoomControllerTests() =>
        _roomController = new RoomController(_roomService, _logger);

    [Theory, AutoData]
    public async Task Should_Get_Rooms(IEnumerable<RoomServiceDto> rooms)
    {
        _roomService.GetRoomsAsync().Returns(rooms);

        var response = await _roomController.GetRoomsAsync();

        Check.That(response.Result).IsInstanceOf<OkObjectResult>();
    }

}