using System.ComponentModel.DataAnnotations;

namespace KataReservation.Api.Dtos.Requests
{
    public class UpdatePersonRequest
    {
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = null!;

        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; } = null!;
    }
}
