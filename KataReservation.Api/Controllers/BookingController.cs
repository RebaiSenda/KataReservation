using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Exceptions;
using KataReservation.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KataReservation.MessagingService.Interfaces;
using KataReservation.MessagingService.Models;
using System.ComponentModel.DataAnnotations;

namespace KataReservation.Api.Controllers;

[Route("api/bookings")]
[ApiController]
[Authorize("KataReservationApiPolicy")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IBookingMessagePublisher _messagePublisher;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        IBookingService bookingService,
        IBookingMessagePublisher messagePublisher,
        ILogger<BookingController> logger)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [EndpointDescription("Créer une réservation")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BookingConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody, Required] CreateBookingRequest request)
    {
        using var scope = _logger.BeginScope("CreateBooking for PersonId: {PersonId}, RoomId: {RoomId}",
            request.PersonId, request.RoomId);

        _logger.LogInformation("Création d'une réservation - Date: {BookingDate}, Créneau: {StartSlot}-{EndSlot}",
            request.BookingDate, request.StartSlot, request.EndSlot);

        try
        {
            var bookingDto = MapToBookingServiceDto(request);
            var result = await _bookingService.CreateBookingAsync(bookingDto);

            _logger.LogInformation("Réservation créée avec succès: ID {BookingId}", result.Id);

            // Publication asynchrone du message (fire-and-forget avec gestion d'erreur)
            _ = Task.Run(async () => await PublishBookingNotificationSafelyAsync(result, "created"));

            var response = MapToBookingResponse(result);
            return CreatedAtAction(nameof(GetBooking), new { id = result.Id }, response);
        }
        catch (BookingConflictException ex)
        {
            _logger.LogWarning("Conflit de réservation: {ErrorMessage}", ex.Message);
            return CreateConflictResponse(ex, request.RoomId, request.BookingDate);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erreur de validation: {ErrorMessage}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de la réservation");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Une erreur interne est survenue lors de la création de la réservation" });
        }
    }

    [HttpGet("{id:int}")]
    [EndpointDescription("Obtenir les détails d'une réservation")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> GetBooking([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "L'ID doit être un entier positif" });
        }

        using var scope = _logger.BeginScope("GetBooking for ID: {BookingId}", id);

        try
        {
            var booking = await _bookingService.GetBookingAsync(id);
            if (booking == null)
            {
                _logger.LogWarning("Réservation non trouvée");
                return NotFound(new { error = $"Aucune réservation trouvée avec l'ID {id}" });
            }

            var response = MapToBookingResponse(booking);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la réservation");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Une erreur interne est survenue lors de la récupération de la réservation" });
        }
    }

    [HttpDelete("{id:int}")]
    [EndpointDescription("Supprimer une réservation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBooking([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "L'ID doit être un entier positif" });
        }

        using var scope = _logger.BeginScope("DeleteBooking for ID: {BookingId}", id);

        try
        {
            var booking = await _bookingService.GetBookingAsync(id);
            if (booking == null)
            {
                _logger.LogWarning("Tentative de suppression d'une réservation inexistante");
                return NotFound(new { error = $"Aucune réservation trouvée avec l'ID {id}" });
            }

            var deleted = await _bookingService.DeleteBookingAsync(id);
            if (!deleted)
            {
                return NotFound(new { error = $"Impossible de supprimer la réservation avec l'ID {id}" });
            }

            // Publication asynchrone du message
            _ = Task.Run(async () => await PublishBookingNotificationSafelyAsync(booking, "deleted"));

            _logger.LogInformation("Réservation supprimée avec succès");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de la réservation");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Une erreur interne est survenue lors de la suppression de la réservation" });
        }
    }

    [HttpPut("{id:int}")]
    [EndpointDescription("Mettre à jour une réservation")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BookingConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> UpdateBooking(
        [FromRoute] int id,
        [FromBody, Required] UpdateBookingRequest request)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "L'ID doit être un entier positif" });
        }

        using var scope = _logger.BeginScope("UpdateBooking for ID: {BookingId}", id);

        _logger.LogInformation("Mise à jour - RoomId: {RoomId}, Date: {BookingDate}, Créneau: {StartSlot}-{EndSlot}",
            request.RoomId, request.BookingDate, request.StartSlot, request.EndSlot);

        try
        {
            var bookingDto = MapToBookingServiceDto(request, id);
            var updatedBooking = await _bookingService.UpdateBookingAsync(bookingDto);

            if (updatedBooking == null)
            {
                _logger.LogWarning("Réservation non trouvée pour mise à jour");
                return NotFound(new { error = $"Aucune réservation trouvée avec l'ID {id}" });
            }

            // Publication asynchrone du message
            _ = Task.Run(async () => await PublishBookingNotificationSafelyAsync(updatedBooking, "updated"));

            var response = MapToBookingResponse(updatedBooking);
            return Ok(response);
        }
        catch (BookingConflictException ex)
        {
            _logger.LogWarning("Conflit lors de la mise à jour: {ErrorMessage}", ex.Message);
            return CreateConflictResponse(ex, request.RoomId, request.BookingDate);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erreur de validation lors de la mise à jour: {ErrorMessage}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de la réservation");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Une erreur interne est survenue lors de la mise à jour de la réservation" });
        }
    }

    #region Méthodes privées

    private static BookingServiceDto MapToBookingServiceDto(CreateBookingRequest request, int id = 0) =>
        new(id, request.RoomId, request.PersonId, request.BookingDate, request.StartSlot, request.EndSlot);

    private static BookingServiceDto MapToBookingServiceDto(UpdateBookingRequest request, int id) =>
        new(id, request.RoomId, request.PersonId, request.BookingDate, request.StartSlot, request.EndSlot);

    private static BookingResponse MapToBookingResponse(BookingServiceDto booking) =>
        new(booking.RoomId, booking.PersonId, booking.BookingDate, booking.StartSlot, booking.EndSlot);

    private ConflictObjectResult CreateConflictResponse(BookingConflictException ex, int roomId, DateTime bookingDate) =>
        Conflict(new BookingConflictResponse(ex.Message, roomId, bookingDate, ex.AvailableSlots));

    private async Task PublishBookingNotificationSafelyAsync(BookingServiceDto booking, string status)
    {
        try
        {
            var notification = new BookingNotificationMessage
            {
                BookingId = booking.Id,
                RoomId = booking.RoomId,
                PersonId = booking.PersonId,
                BookingDate = booking.BookingDate,
                StartSlot = booking.StartSlot,
                EndSlot = booking.EndSlot,
                Status = status
            };

            switch (status.ToLowerInvariant())
            {
                case "created":
                    await _messagePublisher.PublishBookingCreatedAsync(notification);
                    break;
                case "updated":
                    await _messagePublisher.PublishBookingUpdatedAsync(notification);
                    break;
                case "deleted":
                    await _messagePublisher.PublishBookingDeletedAsync(notification);
                    break;
            }

            _logger.LogInformation("Notification {Status} envoyée via RabbitMQ pour la réservation {BookingId}",
                status, booking.Id);
        }
        catch (Exception ex)
        {
            // Ne pas faire échouer l'opération principale si la notification échoue
            _logger.LogError(ex, "Échec de l'envoi de la notification {Status} pour la réservation {BookingId}",
                status, booking.Id);
        }
    }

    #endregion
}