using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using KataReservation.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KataReservation.Dal.Repositories;

public class PersonRepository(KataReservationContext kataReservation) : IPersonRepository
{
    public async Task<IEnumerable<PersonRepositoryDto>> GetPersonsAsync()
    {
        var values = await kataReservation.People.ToListAsync();
        return values.Select(v => new PersonRepositoryDto(v.Id, v.FirstName, v.LastName));
    }

    public async Task<PersonRepositoryDto?> GetPersonByIdAsync(int id)
    {
        var person = await kataReservation.People.FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
        {
            return null;
        }

        return new PersonRepositoryDto(person.Id, person.FirstName, person.LastName);
    }

    public async Task<PersonRepositoryDto> CreatePersonAsync(string firstName, string lastName)
    {
        var personEntity = new PersonEntity { FirstName = firstName, LastName = lastName };
        kataReservation.People.Add(personEntity);
        await kataReservation.SaveChangesAsync();
        return new PersonRepositoryDto(personEntity.Id, personEntity.FirstName, personEntity.LastName);
    }

    public async Task<PersonRepositoryDto?> UpdatePersonAsync(int id, string firstName, string lastName)
    {
        var personEntity = await kataReservation.People.FirstOrDefaultAsync(p => p.Id == id);
        if (personEntity == null)
        {
            return null;
        }
        personEntity.FirstName = firstName;
        personEntity.LastName = lastName;
        await kataReservation.SaveChangesAsync();
        return new PersonRepositoryDto(personEntity.Id, personEntity.FirstName, personEntity.LastName);
    }
    public async Task<bool> DeletePersonAsync(int id)
    {
        // Fetch the person along with related bookings
        var personEntity = await kataReservation.People
            .Include(p => p.Bookings) // Include related bookings
            .FirstOrDefaultAsync(p => p.Id == id);

        if (personEntity == null)
        {
            return false; // Person not found
        }

        // Remove related bookings
        if (personEntity.Bookings.Any())
        {
            kataReservation.Bookings.RemoveRange(personEntity.Bookings);
        }

        // Now remove the person
        kataReservation.People.Remove(personEntity);

        await kataReservation.SaveChangesAsync();
        return true;
    }
}
