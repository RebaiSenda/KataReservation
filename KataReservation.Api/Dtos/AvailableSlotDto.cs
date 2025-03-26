namespace KataReservation.Api.Dtos
{
    public class AvailableSlotDto
    {
        public int StartSlot { get; }
        public int EndSlot { get; }

        public AvailableSlotDto(int startSlot, int endSlot)
        {
            StartSlot = startSlot;
            EndSlot = endSlot;
        }
    }
}
