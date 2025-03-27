using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Exceptions;
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
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BookingConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] CreateBookingRequest request)
    {
        logger.LogInformation("Tentative de création d'une réservation: PersonId {PersonId}, RoomId {RoomId}, Date {BookingDate}, Créneau {StartSlot}-{EndSlot}",
            request.PersonId, request.RoomId, request.BookingDate, request.StartSlot, request.EndSlot);
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
            logger.LogInformation("Réservation créée avec succès: ID {BookingId}", result.Id);
            var response = new BookingResponse(
                result.RoomId,
                result.PersonId,
                result.BookingDate,
                result.StartSlot,
                result.EndSlot
            );
            return CreatedAtAction(nameof(DeleteBooking), new { id = result.Id }, response);
        }
        catch (BookingConflictException ex)
        {
            logger.LogWarning(ex, "Conflit de réservation détecté: {ErrorMessage}", ex.Message);
            return Conflict(new BookingConflictResponse(
                ex.Message,
                request.RoomId,
                request.BookingDate,
                ex.AvailableSlots// Conversion directe de IEnumerable à List
            ));
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Erreur de validation lors de la création d'une réservation: {ErrorMessage}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur inattendue lors de la création d'une réservation");
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue lors de la création de la réservation");
        }
    }

    [HttpGet("{id}")]
    [EndpointDescription("Obtenir les détails d'une réservation")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> GetBooking(int id)
    {
        logger.LogInformation("Tentative de récupération de la réservation avec ID {BookingId}", id);

        try
        {
            var booking = await bookingService.GetBookingAsync(id);

            if (booking == null)
            {
                logger.LogWarning("Réservation non trouvée: ID {BookingId}", id);
                return NotFound();
            }

            var response = new BookingResponse(
                booking.RoomId,
                booking.PersonId,
                booking.BookingDate,
                booking.StartSlot,
                booking.EndSlot
            );

            return Ok(response);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur inattendue lors de la récupération de la réservation avec ID {BookingId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue lors de la récupération de la réservation");
        }
    }

    [HttpDelete("{id}")]
    [EndpointDescription("Supprimer une réservation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        logger.LogInformation("Tentative de suppression de la réservation avec ID {BookingId}", id);

        try
        {
            var result = await bookingService.DeleteBookingAsync(id);

            if (!result)
            {
                logger.LogWarning("Tentative de suppression d'une réservation inexistante: ID {BookingId}", id);
                return NotFound();
            }

            logger.LogInformation("Réservation supprimée avec succès: ID {BookingId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur inattendue lors de la suppression de la réservation avec ID {BookingId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue lors de la suppression de la réservation");
        }
    }
}