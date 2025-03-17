using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Responses;

public record RoomsResponse(IEnumerable<RoomResponse> Rooms)
{
    public RoomsResponse(IEnumerable<RoomServiceDto> Rooms) : this(Rooms.Select(v => new RoomResponse(v))) { }
}