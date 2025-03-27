using KataReservation.Api.Controllers;
using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace KataReservation.Tests.Api.Controllers
{
    public class PersonControllerTests
    {
        private readonly Mock<IPersonService> _mockPersonService;
        private readonly Mock<ILogger<PersonController>> _mockLogger;
        private readonly PersonController _controller;

        public PersonControllerTests()
        {
            _mockPersonService = new Mock<IPersonService>();
            _mockLogger = new Mock<ILogger<PersonController>>();
            _controller = new PersonController(_mockPersonService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetPersonsAsync_ShouldReturnOkResult_WithPersonsResponse()
        {
            var personDtos = new List<PersonServiceDto>
            {
                new(new Person(1, "John", "Doe")),
                new(new Person(2, "Jane", "Smith"))
            };

            _mockPersonService.Setup(service => service.GetPersonsAsync())
                .ReturnsAsync(personDtos);

            var result = await _controller.GetPersonsAsync();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PersonsResponse>(okResult.Value);
            Assert.Equal(2, response.Persons.Count());
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithValidId_ShouldReturnOkResult_WithPersonResponse()
        {
            var personDto = new PersonServiceDto(new Person(1, "John", "Doe"));

            _mockPersonService.Setup(service => service.GetPersonByIdAsync(1))
                .ReturnsAsync(personDto);

            var result = await _controller.GetPersonByIdAsync(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PersonResponse>(okResult.Value);
            Assert.Equal(1, response.Id);
            Assert.Equal("John", response.FirstName);
            Assert.Equal("Doe", response.LastName);
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithInvalidId_ShouldReturnNotFound()
        {
            _mockPersonService.Setup(service => service.GetPersonByIdAsync(999))
                .ReturnsAsync((PersonServiceDto?)null);

            var result = await _controller.GetPersonByIdAsync(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePersonAsync_WithValidRequest_ShouldReturnCreatedAtAction()
        {
            var request = new CreatePersonRequest("John", "Doe");
            var personDto = new PersonServiceDto(new Person(1, "John", "Doe"));

            _mockPersonService.Setup(service => service.CreatePersonAsync("John", "Doe"))
                .ReturnsAsync(personDto);

            var result = await _controller.CreatePersonAsync(request);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(PersonController.GetPersonByIdAsync), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues!["id"]);

            var response = Assert.IsType<PersonResponse>(createdAtActionResult.Value);
            Assert.Equal(1, response.Id);
            Assert.Equal("John", response.FirstName);
            Assert.Equal("Doe", response.LastName);
        }
        [Fact]
        public async Task UpdatePersonAsync_WithValidIdAndRequest_ShouldReturnOkResult()
        {
            var request = new UpdatePersonRequest { FirstName = "John", LastName = "Updated" };
            var personDto = new PersonServiceDto(new Person(1, "John", "Updated"));

            _mockPersonService.Setup(service => service.UpdatePersonAsync(1, "John", "Updated"))
                .ReturnsAsync(personDto);

            var result = await _controller.UpdatePersonAsync(1, request);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PersonResponse>(okResult.Value);
            Assert.Equal(1, response.Id);
            Assert.Equal("John", response.FirstName);
            Assert.Equal("Updated", response.LastName);
        }

        [Fact]
        public async Task UpdatePersonAsync_WithInvalidId_ShouldReturnNotFound()
        {
            var request = new UpdatePersonRequest { FirstName = "John", LastName = "Doe" };

            _mockPersonService.Setup(service => service.UpdatePersonAsync(999, "John", "Doe"))
                .ReturnsAsync((PersonServiceDto?)null);

            var result = await _controller.UpdatePersonAsync(999, request);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdatePersonAsync_WithInvalidModel_ShouldReturnBadRequest()
        {
            var request = new UpdatePersonRequest { FirstName = "", LastName = "" };
            _controller.ModelState.AddModelError("FirstName", "First name is required");

            var result = await _controller.UpdatePersonAsync(1, request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeletePersonAsync_WithValidId_ShouldReturnNoContent()
        {
            _mockPersonService.Setup(service => service.DeletePersonAsync(1))
                .ReturnsAsync(true);

            var result = await _controller.DeletePersonAsync(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePersonAsync_WithInvalidId_ShouldReturnNotFound()
        {
            _mockPersonService.Setup(service => service.DeletePersonAsync(999))
                .ReturnsAsync(false);

            var result = await _controller.DeletePersonAsync(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreatePersonAsync_WithInvalidModel_ShouldReturnBadRequest()
        {
            var request = new CreatePersonRequest("", "");
            _controller.ModelState.AddModelError("FirstName", "First name is required");

            var result = await _controller.CreatePersonAsync(request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}