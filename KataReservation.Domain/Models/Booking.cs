using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KataReservation.Tests")]

namespace KataReservation.Domain.Models;

public record Booking(int Id, int RoomId, int PersonId, DateTime BookingDate, int StartSlot, int EndSlot);