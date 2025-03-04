using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;

namespace KataReservation.Dal.Repositories;

public class PersonRepository : IPersonRepository
{
    private static readonly IList<PersonEntity> Values =
    [
        new PersonEntity
        {
            Id = 100,
            FirstName = "John",
            LastName = "Doe",
        },
        new PersonEntity
        {
            Id = 100,
            FirstName = "Jane",
            LastName = "Doe",
        }
    ];

    public async Task<PersonRepositoryDto?> GetPersonByIdAsync(int id)
    {
        var entity = await Task.FromResult(Values.SingleOrDefault(v => v.Id == id));

        if (entity is null)
        {
            return null;
        }

        return new PersonRepositoryDto(entity.Id, entity.FirstName, entity.LastName);
    }

   
}