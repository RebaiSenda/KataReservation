using System.ComponentModel.DataAnnotations;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Requests;

public class CreateBookingRequest(int id, int RoomId, int PersonId, DateTime BookingDate, int StartSlot, int EndSlot)
{
    public int Id { get; set; } = id;

    public int RoomId { get; set; } = RoomId;

    public int PersonId { get; set; } = PersonId;

    [Required]
    public DateTime BookingDate { get; set; } = BookingDate;

    public int StartSlot { get; set; } = StartSlot; 

    public int EndSlot { get; set; } = EndSlot;

    public BookingServiceDto ToModel() => new(Id, RoomId, PersonId, BookingDate, StartSlot, EndSlot);
}