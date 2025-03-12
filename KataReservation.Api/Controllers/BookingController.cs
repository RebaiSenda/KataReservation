using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KataReservation.Api.Controllers;

[Route("api/Booking")]
[ApiController]
public class BookingController(IBookingService bookingService, ILogger<BookingController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("Lister toutes les réservations")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingsResponse>> GetBookingsAsync()
    {
        logger.Log(LogLevel.Information, "Get Bookings called");
        var bookings = await bookingService.GetBookingsAsync();
        return Ok(new BookingsResponse(bookings));
    }

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

    [HttpGet]
    [Route("bydate/{date}")]
    [EndpointDescription("Obtenir les réservations pour une date spécifique")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingsResponse>> GetBookingsByDateAsync([FromRoute] DateTime date)
    {
        logger.Log(LogLevel.Information, "Get Bookings by Date called with date: {Date}", date);
        var bookings = await bookingService.GetBookingsByDateAsync(date);
        return Ok(new BookingsResponse(bookings));
    }

    [HttpGet]
    [Route("byroom/{roomId}/date/{date}")]
    [EndpointDescription("Obtenir les réservations pour une salle et une date spécifiques")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingsResponse>> GetBookingsByRoomAndDateAsync(
        [FromRoute] int roomId,
        [FromRoute] DateTime date)
    {
        logger.Log(LogLevel.Information, "Get Bookings by Room and Date called with roomId: {RoomId}, date: {Date}", roomId, date);
        var bookings = await bookingService.GetBookingsByRoomAndDateAsync(roomId, date);
        return Ok(new BookingsResponse(bookings));
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

    [HttpGet]
    [Route("availableslots/room/{roomId}/date/{date}")]
    [EndpointDescription("Obtenir les créneaux disponibles pour une salle à une date spécifique")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AvailableSlotsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AvailableSlotsResponse>> GetAvailableSlotsAsync(
    [FromRoute] int roomId,
    [FromRoute] DateTime date)
    {
        logger.Log(LogLevel.Information, "Get Available Slots called with roomId: {RoomId}, date: {Date}", roomId, date);
        var availableSlots = await bookingService.GetAvailableSlotsAsync(roomId, date);
        return Ok(new AvailableSlotsResponse(availableSlots));
    }

    [HttpPost]
    [Route("check")]
    [EndpointDescription("Vérifier la disponibilité d'un créneau et proposer des alternatives si non disponible")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingAvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingAvailabilityResponse>> CheckBookingAvailabilityAsync([FromBody] CheckBookingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.Log(LogLevel.Information, "Check Booking Availability called for roomId: {RoomId}, date: {Date}",
            request.RoomId, request.BookingDate);

        var isAvailable = await bookingService.IsSlotAvailableAsync(
            request.RoomId,
            request.BookingDate,
            request.StartSlot,
            request.EndSlot);

        if (isAvailable)
        {
            return Ok(new BookingAvailabilityResponse(true, null));
        }

        // Si non disponible, récupérer les créneaux libres
        var availableSlots = await bookingService.GetAvailableSlotsAsync(request.RoomId, request.BookingDate);
        return Ok(new BookingAvailabilityResponse(false, availableSlots));
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