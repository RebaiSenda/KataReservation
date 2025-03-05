using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KataReservation.Api.Controllers;

[Authorize("KataReservationApiPolicy")]
[Route("api/Room")]
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

}