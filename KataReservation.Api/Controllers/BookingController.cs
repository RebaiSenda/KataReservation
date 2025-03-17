using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace KataReservation.Api.Controllers;

[Route("api/bookings")]
[ApiController]
public class BookingController(IBookingService bookingService, ILogger<BookingController> logger) : ControllerBase
{
 
    [HttpGet]
    [Route("{id}")]
    [EndpointDescription("Obtenir une réservation par son ID")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> GetBookingByIdAsync([FromRoute] int id)
    {
        logger.Log(LogLevel.Information, "Get Booking by ID called with ID: {Id}", id);
        var booking = await bookingService.GetBookingByIdAsync(id);
        if (booking is null)
        {
            return NotFound();
        }
        return Ok(new BookingResponse(booking));
    }

    [HttpPost]
    [EndpointDescription("Créer une réservation")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> CreateBookingAsync([FromBody] CreateBookingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.Log(LogLevel.Information, "Create Booking called");
        var booking = await bookingService.CreateBookingAsync(request.ToModel());

        return CreatedAtAction(
            nameof(GetBookingByIdAsync),
            new { id = booking.Id },
            new BookingResponse(booking)
        );
    }
    [HttpDelete("{id}")]
    [EndpointDescription("Supprimer une réservation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBookingAsync([FromRoute] int id)
    {
        logger.Log(LogLevel.Information, "Delete Booking called with ID: {Id}", id);
        var isDeleted = await bookingService.DeleteBookingAsync(id);

        if (!isDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}