using System.ComponentModel.DataAnnotations;

namespace KataReservation.Api.Dtos.Requests
{
    public class CreatePersonRequest(string firstName, string lastName)
    {
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = firstName!;
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; } = lastName!;
    }
}
