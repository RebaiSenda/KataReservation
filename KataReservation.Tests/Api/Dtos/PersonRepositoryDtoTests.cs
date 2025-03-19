using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Dtos.Services;
using NFluent;

namespace KataReservation.Tests.Api.Dtos
{
    public class PersonRepositoryDtoCaseTests
    {
        [Fact]
        public void Should_Capitalize_FirstName_First_Letter()
        {
            // Arrange
            string lowercaseName = "john";

            // Act
            var person = new PersonRepositoryDtoWithCase(1, lowercaseName, "Doe");

            // Assert
            Check.That(person.FirstName).IsEqualTo("John");
        }

        [Fact]
        public void Should_Capitalize_LastName_First_Letter()
        {
            // Arrange
            string lowercaseName = "doe";

            // Act
            var person = new PersonRepositoryDtoWithCase(1, "John", lowercaseName);

            // Assert
            Check.That(person.LastName).IsEqualTo("Doe");
        }

        [Fact]
        public void Should_Preserve_FirstName_When_Already_Capitalized()
        {
            // Arrange
            string properName = "John";

            // Act
            var person = new PersonRepositoryDtoWithCase(1, properName, "Doe");

            // Assert
            Check.That(person.FirstName).IsEqualTo("John");
        }

        [Fact]
        public void Should_Handle_Empty_FirstName()
        {
            // Arrange
            string emptyName = "";

            // Act
            var person = new PersonRepositoryDtoWithCase(1, emptyName, "Doe");

            // Assert
            Check.That(person.FirstName).IsEqualTo("");
        }

        [Fact]
        public void Should_Handle_Empty_LastName()
        {
            // Arrange
            string emptyName = "";

            // Act
            var person = new PersonRepositoryDtoWithCase(1, "John", emptyName);

            // Assert
            Check.That(person.LastName).IsEqualTo("");
        }

        [Fact]
        public void Should_Handle_Null_FirstName()
        {
            // Arrange
            string nullName = null!;

            // Act & Assert
            Check.ThatCode(() => new PersonRepositoryDtoWithCase(1, nullName, "Doe"))
                .DoesNotThrow();
        }

        [Fact]
        public void Should_Capitalize_First_Letter_From_ServiceDto()
        {
            // Arrange
            var serviceDto = new PersonServiceDto(42, "jane", "smith");

            // Act
            var repositoryDto = new PersonRepositoryDtoWithCase(serviceDto);

            // Assert
            Check.That(repositoryDto.FirstName).IsEqualTo("Jane");
            Check.That(repositoryDto.LastName).IsEqualTo("Smith");
        }
    }

}