using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Responses
{
    public record BookingConflictResponse(
        string Message,
        int RoomId,
        DateTime Date,
        IEnumerable<SlotDto> AvailableSlots
    )
    {
        // Constructeur avec initialisateur 'this'
        public BookingConflictResponse(string message, int roomId, DateTime bookingDate, object availableSlots)
            : this(
                message,
                roomId,
                bookingDate,
                availableSlots is IEnumerable<SlotDto> slots
                    ? slots
                    : new List<SlotDto>()
            )
        {
            // Le constructeur est maintenant correct avec l'initialisateur 'this'
        }
    }
}