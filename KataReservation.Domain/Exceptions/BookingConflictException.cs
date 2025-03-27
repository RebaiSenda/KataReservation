using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Exceptions
{

    public class BookingConflictException : Exception
    {
        public List<SlotDto> AvailableSlots { get; }

        public BookingConflictException(string message, List<SlotDto> availableSlots)
            : base(message)
        {
            AvailableSlots = availableSlots;
        }
    }
}