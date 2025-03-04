using NFluent;
using KataReservation.Domain.Models;

namespace KataReservation.Tests.Domain.Models;

public class BookingTests
{
    [Fact]
    public void Shoud_Create_Value()
    {
        const int id = 100;
        const int idRoom = 100;
        const int idPerson = 100;

        var value = new Booking(id, idRoom, idPerson,DateTime.Now,1,1);

        Check.That(value.Id).Is(id);
        Check.That(value.RoomId).Is(idRoom);
    }
}