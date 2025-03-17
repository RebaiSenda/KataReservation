using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace KataReservation.Api.Controllers;

[Route("api/bookings")]
[ApiController]
public class BookingController(IBookingService bookingService, ILogger<BookingController> logger) : ControllerBase
{
    [HttpPost]
    [EndpointDescription("Créer une réservation")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            var bookingDto = new BookingServiceDto(
                0,
                request.RoomId,
                request.PersonId,
                request.BookingDate,
            request.StartSlot,
                request.EndSlot
            );

            var result = await bookingService.CreateBookingAsync(bookingDto);

            var response = new BookingResponse(
                result.Id,
                result.RoomId,
                result.PersonId,
                result.BookingDate,
                result.StartSlot,
                result.EndSlot
            );

            return CreatedAtAction(nameof(CreateBooking), new { id = result.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [EndpointDescription("Supprimer une réservation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var result = await bookingService.DeleteBookingAsync(id);

        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }  
}