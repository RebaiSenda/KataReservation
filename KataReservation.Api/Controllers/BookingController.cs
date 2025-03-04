using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KataReservation.Api.Controllers;

[Authorize("KataReservationApiPolicy")]
[Route("api/booking")]
[ApiController]
public class BookingController(IBookingService bookingService, ILogger<BookingController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("Get Booking")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingsResponse>> GetBookingsAsync()
    {
        logger.Log(LogLevel.Information, "Get Booking called");

        var values = await bookingService.GetBookingsAsync();
        return Ok(new BookingsResponse(values));
    }

    [HttpGet]
    [Route("{id}")]
    [EndpointDescription("Get Booking by id")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> GetBookingByIdAsync([FromRoute] int id)
    {
        var value = await bookingService.GetBookingByIdAsync(id);

        if (value is null)
        {
            return NotFound();
        }

        return Ok(new BookingResponse(value));
    }

    [HttpPost]
    [EndpointDescription("Create a new Booking")]
    [Consumes(typeof(CreateBookingRequest), "application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> CreateBookingAsync([FromBody] CreateBookingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var booking = await bookingService.CreateBookingAsync(request.ToModel());
        return CreatedAtAction(nameof(GetBookingByIdAsync), new { booking.Id }, booking);
    }

    [HttpPut("{id}")]
    [EndpointDescription("Update a booking by id")]
    [Consumes(typeof(UpdateBookingRequest), "application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> UpdateBookingAsync([FromRoute] int id, [FromBody] UpdateBookingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var value = await bookingService.UpdateBookingAsync(request.ToModel(id));

        if (value is null)
        {
            return NotFound();
        }

        return Ok(new BookingResponse(value));
    }

    [HttpDelete("{id}")]
    [EndpointDescription("Delete a booking by id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBookingAsync([FromRoute] int id)
    {
        var isDeleted = await bookingService.DeleteBookingAsync(id);

        if (!isDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}