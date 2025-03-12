using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Responses
{
    public record AvailableSlotResponse(int StartSlot, int EndSlot);

    public record AvailableSlotsResponse
    {
        public List<AvailableSlotResponse> AvailableSlots { get; }

        public AvailableSlotsResponse(List<AvailableSlotDto> availableSlots)
        {
            AvailableSlots = availableSlots.Select(slot =>
                new AvailableSlotResponse(slot.StartSlot, slot.EndSlot)).ToList();
        }
    }
    public record BookingAvailabilityResponse(bool IsAvailable, List<AvailableSlotDto> AvailableSlots);
}
