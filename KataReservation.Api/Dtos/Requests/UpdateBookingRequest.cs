namespace KataReservation.Api.Dtos.Requests
{
    public class UpdateBookingRequest
    {
        public UpdateBookingRequest(int RoomId, int PersonId, DateTime BookingDate, int StartSlot, int EndSlot)
        {
            this.RoomId = RoomId;
            this.PersonId = PersonId;
            this.BookingDate = BookingDate;
            this.StartSlot = StartSlot;
            this.EndSlot = EndSlot;
        }

        public int RoomId { get; set; }
        public int PersonId { get; set; }
        public DateTime BookingDate { get; set; }
        public int StartSlot { get; set; }
        public int EndSlot { get; set; }
    }
}
