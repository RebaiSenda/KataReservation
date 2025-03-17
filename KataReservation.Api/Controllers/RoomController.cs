using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using RoomResponse = KataReservation.Api.Dtos.Responses.RoomResponse;
using RoomsResponse = KataReservation.Api.Dtos.Responses.RoomsResponse;

namespace KataReservation.Api.Controllers;

[Route("api/rooms")]
[ApiController]
public class RoomController(IRoomService RoomService, ILogger<RoomController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("Lister des salles")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomsResponse>> GetRoomsAsync()
    {
        logger.Log(LogLevel.Information, "Get Room called");

        var values = await RoomService.GetRoomsAsync();
        return Ok(new RoomsResponse(values));
    }

    [HttpGet("{id}")]
    [EndpointDescription("Obtenir une salle par son ID")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomResponse>> GetRoomByIdAsync(int id)
    {
        logger.Log(LogLevel.Information, "Get Room by ID called with ID: {Id}", id);
        var room = await RoomService.GetRoomByIdAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        return Ok(room);
    }
    [HttpPost]
    [EndpointDescription("Créer une nouvelle salle")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomResponse>> CreateRoomAsync(CreateRoomRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.Log(LogLevel.Information, "Create Room called");
        var createdRoom = await RoomService.CreateRoomAsync(request.RoomName);

        return CreatedAtAction(nameof(GetRoomByIdAsync), new { id = createdRoom.Id }, new RoomResponse(createdRoom));
    }

    [HttpPut("{id}")]
    [EndpointDescription("Mettre à jour une salle")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomResponse>> UpdateRoomAsync(int id, UpdateRoomRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.Log(LogLevel.Information, "Update Room called with ID: {Id}", id);
        var updatedRoom = await RoomService.UpdateRoomAsync(id, request.RoomName);

        if (updatedRoom == null)
        {
            return NotFound();
        }

        return Ok(new RoomResponse(updatedRoom));
    }

    [HttpDelete("{id}")]
    [EndpointDescription("Supprimer une salle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteRoomAsync(int id)
    {
        logger.Log(LogLevel.Information, "Delete Room called with ID: {Id}", id);
        var result = await RoomService.DeleteRoomAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

}
