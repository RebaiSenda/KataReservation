using System.ComponentModel.DataAnnotations;
using NFluent;
using KataReservation.Api.Dtos.Requests;

namespace KataReservation.Tests.Api.Dtos;

public class UpdateValueRequestTests
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

    [Fact]
    public void Should_Not_Valid_Request_When_No_Name()
    {
        var validRequest = CreateValidRequest();
        var request = new UpdateBookingRequest(11, 1, DateTime.Now, 2, 1);
        var validationContext = new ValidationContext(request);
        var errors = new List<ValidationResult>();

        var result = Validator.TryValidateObject(request, validationContext, errors, true);

        Check.That(result).IsFalse();
        Check.That(errors).HasSize(1);
        Check.That(errors.Single().MemberNames).HasSize(1);
        Check.That(errors.Single().MemberNames.Single()).IsEqualTo(nameof(UpdateBookingRequest.RoomId));
    }

    private static UpdateBookingRequest CreateValidRequest() => new(11,1,DateTime.Now,2,1);
}