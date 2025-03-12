using System.ComponentModel.DataAnnotations;
using NFluent;
using KataReservation.Api.Dtos.Requests;

namespace KataReservation.Tests.Api.Dtos;

public class CreateValueRequestTests
{
    [Fact]
    public void Should_Valid_Request()
    {
        var request = CreateValidRequest();
        var validationContext = new ValidationContext(request);
        var errors = new List<ValidationResult>();

        var result = Validator.TryValidateObject(request, validationContext, errors, true);

        Check.That(result).IsTrue();
        Check.That(errors).HasSize(0);
    }


    private static CreateBookingRequest CreateValidRequest() => new(1,1,1,DateTime.Now,1,1);
}