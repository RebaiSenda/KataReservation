using System.ComponentModel.DataAnnotations;

namespace KataReservation.Api.Dtos.Requests
{
    public record CreateRoomRequest([Required] string RoomName);
}
