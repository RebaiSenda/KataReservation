using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Exceptions;
using KataReservation.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KataReservation.MessagingService.Interfaces;
using KataReservation.MessagingService.Models;

namespace KataReservation.Api.Controllers;

[Route("api/bookings")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        IBookingService bookingService,
        IMessagePublisher messagePublisher, // Utiliser IMessagePublisher au lieu de IMessagingService
        ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    [HttpPost]
    [EndpointDescription("Créer une réservation")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BookingConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize("KataReservationApiPolicy")]
    public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] CreateBookingRequest request)
    {
        _logger.LogInformation("Tentative de création d'une réservation: PersonId {PersonId}, RoomId {RoomId}, Date {BookingDate}, Créneau {StartSlot}-{EndSlot}",
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
            var result = await _bookingService.CreateBookingAsync(bookingDto);
            _logger.LogInformation("Réservation créée avec succès: ID {BookingId}", result.Id);

            // Créer le message pour RabbitMQ
            var bookingNotification = new BookingNotificationMessage
            {
                BookingId = result.Id,
                RoomId = result.RoomId,
                PersonId = result.PersonId,
                BookingDate = result.BookingDate,
                StartSlot = result.StartSlot,
                EndSlot = result.EndSlot,
                Status = "Created"
            };

            // Publier le message à travers RabbitMQ
            await _messagePublisher.PublishBookingCreatedAsync(bookingNotification);
            _logger.LogInformation("Notification de réservation envoyée via RabbitMQ: ID {BookingId}", result.Id);

            var response = new BookingResponse(
                result.RoomId,
                result.PersonId,
                result.BookingDate,
                result.StartSlot,
                result.EndSlot
            );
            return CreatedAtAction(nameof(GetBooking), new { id = result.Id }, response);
        }
        catch (BookingConflictException ex)
        {
            _logger.LogWarning(ex, "Conflit de réservation détecté: {ErrorMessage}", ex.Message);
            return Conflict(new BookingConflictResponse(
                ex.Message,
                request.RoomId,
                request.BookingDate,
                ex.AvailableSlots
            ));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erreur de validation lors de la création d'une réservation: {ErrorMessage}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors de la création d'une réservation");
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue lors de la création de la réservation");
        }
    }

    [HttpGet("{id}")]
    [EndpointDescription("Obtenir les détails d'une réservation")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize("KataReservationApiPolicy")]
    public async Task<ActionResult<BookingResponse>> GetBooking(int id)
    {
        _logger.LogInformation("Tentative de récupération de la réservation avec ID {BookingId}", id);

        try
        {
            var booking = await _bookingService.GetBookingAsync(id);

            if (booking == null)
            {
                _logger.LogWarning("Réservation non trouvée: ID {BookingId}", id);
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
            _logger.LogError(ex, "Erreur inattendue lors de la récupération de la réservation avec ID {BookingId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue lors de la récupération de la réservation");
        }
    }

    [HttpDelete("{id}")]
    [EndpointDescription("Supprimer une réservation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize("KataReservationApiPolicy")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        _logger.LogInformation("Tentative de suppression de la réservation avec ID {BookingId}", id);

        try
        {
            // Récupérer d'abord les détails de la réservation avant de la supprimer
            var booking = await _bookingService.GetBookingAsync(id);
            if (booking == null)
            {
                _logger.LogWarning("Tentative de suppression d'une réservation inexistante: ID {BookingId}", id);
                return NotFound();
            }

            // Supprimer la réservation
            var result = await _bookingService.DeleteBookingAsync(id);
            if (!result)
            {
                _logger.LogWarning("Échec de la suppression de la réservation: ID {BookingId}", id);
                return NotFound();
            }

            // Créer le message pour RabbitMQ avec les détails complets de la réservation
            var bookingNotification = new BookingNotificationMessage
            {
                BookingId = id,
                RoomId = booking.RoomId,
                PersonId = booking.PersonId,
                BookingDate = booking.BookingDate,
                StartSlot = booking.StartSlot,
                EndSlot = booking.EndSlot,
                Status = "Deleted"
            };

            // Publier le message de suppression via RabbitMQ
            await _messagePublisher.PublishBookingDeletedAsync(bookingNotification);
            _logger.LogInformation("Notification de suppression de réservation envoyée via RabbitMQ: ID {BookingId}", id);

            _logger.LogInformation("Réservation supprimée avec succès: ID {BookingId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors de la suppression de la réservation avec ID {BookingId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue lors de la suppression de la réservation");
        }
    }

    [HttpPut("{id}")]
    [EndpointDescription("Mettre à jour une réservation")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BookingConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize("KataReservationApiPolicy")]
    public async Task<ActionResult<BookingResponse>> UpdateBooking(int id, [FromBody] UpdateBookingRequest request)
    {
        _logger.LogInformation("Tentative de mise à jour de la réservation: ID {BookingId}, RoomId {RoomId}, Date {BookingDate}, Créneau {StartSlot}-{EndSlot}",
            id, request.RoomId, request.BookingDate, request.StartSlot, request.EndSlot);

        try
        {
            var bookingDto = new BookingServiceDto(
                id,
                request.RoomId,
                request.PersonId,
                request.BookingDate,
                request.StartSlot,
                request.EndSlot
            );

            var updatedBooking = await _bookingService.UpdateBookingAsync(bookingDto);
            if (updatedBooking == null)
            {
                _logger.LogWarning("Réservation non trouvée pour mise à jour: ID {BookingId}", id);
                return NotFound();
            }

            // Créer le message pour RabbitMQ
            var bookingNotification = new BookingNotificationMessage
            {
                BookingId = updatedBooking.Id,
                RoomId = updatedBooking.RoomId,
                PersonId = updatedBooking.PersonId,
                BookingDate = updatedBooking.BookingDate,
                StartSlot = updatedBooking.StartSlot,
                EndSlot = updatedBooking.EndSlot,
                Status = "Updated"
            };

            // Publier le message à travers RabbitMQ
            await _messagePublisher.PublishBookingUpdatedAsync(bookingNotification);
            _logger.LogInformation("Notification de mise à jour de réservation envoyée via RabbitMQ: ID {BookingId}", id);

            var response = new BookingResponse(
                updatedBooking.RoomId,
                updatedBooking.PersonId,
                updatedBooking.BookingDate,
                updatedBooking.StartSlot,
                updatedBooking.EndSlot
            );

            return Ok(response);
        }
        catch (BookingConflictException ex)
        {
            _logger.LogWarning(ex, "Conflit de réservation détecté lors de la mise à jour: {ErrorMessage}", ex.Message);
            return Conflict(new BookingConflictResponse(
                ex.Message,
                request.RoomId,
                request.BookingDate,
                ex.AvailableSlots
            ));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erreur de validation lors de la mise à jour d'une réservation: {ErrorMessage}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors de la mise à jour de la réservation avec ID {BookingId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue lors de la mise à jour de la réservation");
        }
    }
}