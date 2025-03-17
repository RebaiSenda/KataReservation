using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace KataReservation.Api.Controllers
{
    [Route("api/persons")]
    [ApiController]
    public class PersonController(IPersonService personService, ILogger<PersonController> logger) : ControllerBase
    {
        [HttpGet]
        [EndpointDescription("Lister les personnes")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PersonsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonsResponse>> GetPersonsAsync()
        {
            logger.Log(LogLevel.Information, "Get Persons called");
            var values = await personService.GetPersonsAsync();
            return Ok(new PersonsResponse(values));
        }

        [HttpGet("{id}")]
        [EndpointDescription("Obtenir une personne par son ID")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonResponse>> GetPersonByIdAsync(int id)
        {
            logger.Log(LogLevel.Information, "Get Person by ID called with ID: {Id}", id);
            var person = await personService.GetPersonByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(new PersonResponse(person));
        }

        [HttpPost]
        [EndpointDescription("Créer une nouvelle personne")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonResponse>> CreatePersonAsync(CreatePersonRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            logger.Log(LogLevel.Information, "Create Person called");
            var createdPerson = await personService.CreatePersonAsync(request.FirstName, request.LastName);
            return CreatedAtAction(nameof(GetPersonByIdAsync), new { id = createdPerson.Id }, new PersonResponse(createdPerson));
        }

        [HttpPut("{id}")]
        [EndpointDescription("Mettre à jour une personne")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonResponse>> UpdatePersonAsync(int id, UpdatePersonRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            logger.Log(LogLevel.Information, "Update Person called with ID: {Id}", id);
            var updatedPerson = await personService.UpdatePersonAsync(id, request.FirstName, request.LastName);
            if (updatedPerson == null)
            {
                return NotFound();
            }
            return Ok(new PersonResponse(updatedPerson));
        }

        [HttpDelete("{id}")]
        [EndpointDescription("Supprimer une personne")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePersonAsync(int id)
        {
            logger.Log(LogLevel.Information, "Delete Person called with ID: {Id}", id);
            var result = await personService.DeletePersonAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
