using NFluent;
using KataReservation.Domain.Models;

namespace KataReservation.Tests.Domain.Models;

public class RoomTests
{
    [Fact]
    public void Shoud_Create_Room()
    {
        const int id = 100;
        const string roomName = "R1";

        var value = new Room(id, roomName);

        Check.That(value.Id).Is(id);
        Check.That(value.RoomName).Is(roomName);
    }
}