using System.ComponentModel.DataAnnotations;
using KataReservation.Api.Dtos.Requests;
using Xunit;

namespace KataReservation.Tests.Api.Dtos
{
    public class CreatePersonRequestTests
    {
        [Fact]
        public void Should_Valid_Request()
        {
            // Arrange
            var request = new CreatePersonRequest("John", "Doe");

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void Should_Not_Valid_Request_When_First_Name_Is_Greater_Than_50()
        {
            // Arrange
            var longFirstName = new string('A', 51);
            var request = new CreatePersonRequest(longFirstName, "Doe");

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Single(validationResults);
            Assert.Contains(validationResults,
                v => v.MemberNames.Contains(nameof(CreatePersonRequest.FirstName)) &&
                     v.ErrorMessage == $"The field FirstName must be a string with a minimum length of 2 and a maximum length of 50.");
        }

        [Fact]
        public void Should_Not_Valid_Request_When_First_Name_Is_Lower_Than_2()
        {
            // Arrange
            var request = new CreatePersonRequest("A", "Doe");

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Single(validationResults);
            Assert.Contains(validationResults,
                v => v.MemberNames.Contains(nameof(CreatePersonRequest.FirstName)) &&
                     v.ErrorMessage == $"The field FirstName must be a string with a minimum length of 2 and a maximum length of 50.");
        }

        [Fact]
        public void Should_Not_Valid_Request_When_Last_Name_Is_Greater_Than_50()
        {
            // Arrange
            var longLastName = new string('B', 51);
            var request = new CreatePersonRequest("John", longLastName);

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Single(validationResults);
            Assert.Contains(validationResults,
                v => v.MemberNames.Contains(nameof(CreatePersonRequest.LastName)) &&
                     v.ErrorMessage == $"The field LastName must be a string with a minimum length of 2 and a maximum length of 50.");
        }

        [Fact]
        public void Should_Not_Valid_Request_When_Last_Name_Is_Lower_Than_2()
        {
            // Arrange
            var request = new CreatePersonRequest("John", "A");

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            Assert.Single(validationResults);
            Assert.Contains(validationResults,
                v => v.MemberNames.Contains(nameof(CreatePersonRequest.LastName)) &&
                     v.ErrorMessage == $"The field LastName must be a string with a minimum length of 2 and a maximum length of 50.");
        }

        // Helper method to validate model
        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}