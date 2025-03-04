using System.ComponentModel.DataAnnotations;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Requests;

public class CreateBookingRequest(int id, int RoomId, int PersonId, DateTime BookingDate, int StartSlot, int EndSlot)
{
    [Range(1, 1_000)]
    public int Id { get; set; } = id;

    [Range(1, 1_000)]
    public int RoomId { get; set; } = RoomId;

    [Range(1, 1_000)]
    public int PersonId { get; set; } = PersonId;

    [Required]
    public DateTime BookingDate { get; set; } = BookingDate;

    [Range(1, 1_000)]
    public int StartSlot { get; set; } = StartSlot; 

    [Range(1, 1_000)]
    public int EndSlot { get; set; } = EndSlot;

    public BookingServiceDto ToModel() => new(Id, RoomId, PersonId, BookingDate, StartSlot, EndSlot);
}