using System.ComponentModel.DataAnnotations;

namespace KataReservation.Api.Dtos.Requests
{
    public record UpdateRoomRequest([Required] string RoomName);
}
