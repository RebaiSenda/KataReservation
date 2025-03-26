using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Responses
{
    public record AvailableSlotResponse(int StartSlot, int EndSlot);

    public record AvailableSlotsResponse
    {
        public List<AvailableSlotResponse> AvailableSlots { get; init; }

        public AvailableSlotsResponse(List<AvailableSlotDto> availableSlots)
        {
            AvailableSlots = [.. availableSlots.Select(slot =>
            new AvailableSlotResponse(slot.StartSlot, slot.EndSlot))];
        }
    }
    public record BookingAvailabilityResponse(bool IsAvailable, List<AvailableSlotDto> AvailableSlots);
}
