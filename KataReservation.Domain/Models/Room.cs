using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KataReservation.Tests")]

namespace KataReservation.Domain.Models;

public record Room(int Id, string RoomName)
{
    internal static void UpdateName(string v)
    {
        throw new NotImplementedException();
    }
}
