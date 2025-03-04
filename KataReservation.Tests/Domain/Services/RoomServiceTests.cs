using AutoFixture.Xunit2;
using NFluent;
using NSubstitute;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Services;

namespace KataReservation.Tests.Domain.Services;

public class RoomServiceTests
{
    private readonly RoomService _roomService;
    private readonly IRoomRepository _roomRepository = Substitute.For<IRoomRepository>();

    public RoomServiceTests() =>
        _roomService = new RoomService(_roomRepository);

    [Theory, AutoData]
    public async Task Shoud_Get_Values(IEnumerable<RoomRepositoryDto> values)
    {
        _roomRepository.GetRoomsAsync().Returns(values);

        var result = await _roomService.GetRoomsAsync();

        Check.That(result).IsNotNull();
    }

}