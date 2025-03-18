using System.ComponentModel.DataAnnotations;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Requests;

public record CreateBookingRequest(int RoomId, int PersonId, DateTime BookingDate, int StartSlot, int EndSlot);