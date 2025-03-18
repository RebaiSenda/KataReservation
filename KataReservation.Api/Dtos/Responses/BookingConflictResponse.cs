using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Responses
{
    public record BookingConflictResponse(
    string Message,
    int RoomId,
    DateTime Date,
    IEnumerable<SlotDto> AvailableSlots
);
}
