using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Api.Dtos.Responses
{
    public record PersonResponse(int Id, string FirstName, string LastName)
    {
        public PersonResponse(PersonServiceDto dto) : this(dto.Id, dto.FirstName, dto.LastName) { }
    }

    public record PersonsResponse(IEnumerable<PersonResponse> Persons)
    {
        public PersonsResponse(IEnumerable<PersonServiceDto> Persons) : this(Persons.Select(v => new PersonResponse(v))) { }
    }
}
