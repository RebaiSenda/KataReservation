using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Services;
using Moq;

namespace KataReservation.Tests.Domain.Services
{
    public class PersonServiceTest
    {
        private readonly Mock<IPersonRepository> _mockPersonRepository;
        private readonly PersonService _personService;

        public PersonServiceTest()
        {
            _mockPersonRepository = new Mock<IPersonRepository>();
            _personService = new PersonService(_mockPersonRepository.Object);
        }

        [Fact]
        public async Task GetPersonsAsync_ShouldReturnAllPersons()
        {
                var personList = new List<PersonRepositoryDto>
        {
            new PersonRepositoryDto(1, "John", "Doe"),
            new PersonRepositoryDto(2, "Jane", "Smith")
        };

            _mockPersonRepository.Setup(repo => repo.GetPersonsAsync())
                .ReturnsAsync(personList);

            var result = await _personService.GetPersonsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var personsList = result.ToList();
            Assert.Equal(1, personsList[0].Id);
            Assert.Equal("John", personsList[0].FirstName);
            Assert.Equal("Doe", personsList[0].LastName);

            Assert.Equal(2, personsList[1].Id);
            Assert.Equal("Jane", personsList[1].FirstName);
            Assert.Equal("Smith", personsList[1].LastName);
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithValidId_ShouldReturnPerson()
        {
            var person = new PersonRepositoryDto(1, "John", "Doe");

            _mockPersonRepository.Setup(repo => repo.GetPersonByIdAsync(1))
                .ReturnsAsync(person);

            var result = await _personService.GetPersonByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            _mockPersonRepository.Setup(repo => repo.GetPersonByIdAsync(999))
                .ReturnsAsync((PersonRepositoryDto)null!);

            var result = await _personService.GetPersonByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreatePersonAsync_ShouldReturnCreatedPerson()
        {
            var person = new PersonRepositoryDto(1, "John", "Doe");

            _mockPersonRepository.Setup(repo => repo.CreatePersonAsync("John", "Doe"))
                .ReturnsAsync(person);

            var result = await _personService.CreatePersonAsync("John", "Doe");

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
        }

        [Fact]
        public async Task UpdatePersonAsync_WithValidId_ShouldReturnUpdatedPerson()
        {
            var updatedPerson = new PersonRepositoryDto(1, "John", "Updated");

            _mockPersonRepository.Setup(repo => repo.UpdatePersonAsync(1, "John", "Updated"))
                .ReturnsAsync(updatedPerson);

            var result = await _personService.UpdatePersonAsync(1, "John", "Updated");

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Updated", result.LastName);
        }
        [Fact]
        public async Task UpdatePersonAsync_WithInvalidId_ShouldReturnNull()
        {
            _mockPersonRepository.Setup(repo => repo.UpdatePersonAsync(999, "John", "Doe"))
                .ReturnsAsync((PersonRepositoryDto)null!);

            var result = await _personService.UpdatePersonAsync(999, "John", "Doe");

            Assert.Null(result);
        }

        [Fact]
        public async Task DeletePersonAsync_WithValidId_ShouldReturnTrue()
        {
            _mockPersonRepository.Setup(repo => repo.DeletePersonAsync(1))
                .ReturnsAsync(true);

            var result = await _personService.DeletePersonAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeletePersonAsync_WithInvalidId_ShouldReturnFalse()
        {
            _mockPersonRepository.Setup(repo => repo.DeletePersonAsync(999))
                .ReturnsAsync(false);

            var result = await _personService.DeletePersonAsync(999);

            Assert.False(result);
        }
    }
}