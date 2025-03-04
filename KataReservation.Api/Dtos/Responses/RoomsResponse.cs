using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Responses;

public record RoomsResponse(IEnumerable<RoomResponse> Values)
{
    public RoomsResponse(IEnumerable<RoomServiceDto> values) : this(values.Select(v => new RoomResponse(v))) { }
}