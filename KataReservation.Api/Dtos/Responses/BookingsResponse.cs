using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Responses;

public record BookingsResponse(IEnumerable<BookingResponse> Values)
{
    public BookingsResponse(IEnumerable<BookingServiceDto> values) : this(values.Select(v => new BookingResponse(v))) { }
}