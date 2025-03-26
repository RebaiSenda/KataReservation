namespace KataReservation.Domain.Exceptions
{
    public class BookingConflictException : InvalidOperationException
    {
        public BookingConflictException()
            : base("Un conflit a été détecté sur ce créneau horaire.")
        {
        }

        public BookingConflictException(string message)
            : base(message)
        {
        }

        public BookingConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}