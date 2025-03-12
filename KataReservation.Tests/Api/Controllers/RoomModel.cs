using KataReservation.Domain.Dtos.Repositories;

namespace KataReservation.Tests.Api.Controllers
{
    internal record RoomModel(int Id, string RoomName) : RoomServiceDto(Id, RoomName)
    {
    }
}