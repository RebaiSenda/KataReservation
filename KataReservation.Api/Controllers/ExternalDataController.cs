using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KataReservation.Api.Controllers
{
    [ApiController]
    [Route("api/external-data")]
    [Authorize("KataReservationApiPolicy")]
    public class ExternalDataController : ControllerBase
    {
        private readonly ISimpleDataService _dataService;
        private readonly ILogger<ExternalDataController> _logger;

        public ExternalDataController(
            ISimpleDataService dataService,
            ILogger<ExternalDataController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SimpleDataResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SimpleDataResponse>> GetExternalData()
        {
            _logger.LogInformation("Récupération des données depuis KataSimpleAPI");

            try
            {
                var data = await _dataService.GetExternalDataAsync();
                return Ok(data);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Authentification requise pour accéder aux données externes" });
            }
           
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des données externes");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur est survenue lors de la récupération des données externes" });
            }
        }
    }
}