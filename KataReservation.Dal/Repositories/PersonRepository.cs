using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KataReservation.Dal.Repositories;

public class PersonRepository(KataReservationContext kataReservation) : IPersonRepository
{
    

    public async Task<PersonRepositoryDto?> GetPersonByIdAsync(int id)
    {
        var entity = await kataReservation.People.SingleOrDefaultAsync(v => v.Id == id);

        if (entity is null)
        {
            return null;
        }

        return new PersonRepositoryDto(entity.Id, entity.FirstName, entity.LastName);
    }

   
}