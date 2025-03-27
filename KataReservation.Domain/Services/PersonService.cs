using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KataReservation.Domain.Dtos.Services;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Models;

namespace KataReservation.Domain.Services
{
    public class PersonService(IPersonRepository personRepository) : IPersonService
    {
        public async Task<IEnumerable<PersonServiceDto>> GetPersonsAsync()
        {
            var persons = await personRepository.GetPersonsAsync();
            return persons.Select(p => new PersonServiceDto(new Person(p.Id, p.FirstName, p.LastName)));
        }

        public async Task<PersonServiceDto?> GetPersonByIdAsync(int id)
        {
            var person = await personRepository.GetPersonByIdAsync(id);
            if (person == null)
            {
                return null!;
            }
            return new PersonServiceDto(new Person(person.Id, person.FirstName, person.LastName));
        }

        public async Task<PersonServiceDto> CreatePersonAsync(string firstName, string lastName)
        {
            var person = await personRepository.CreatePersonAsync(firstName, lastName);
            return new PersonServiceDto(new Person(person.Id, person.FirstName, person.LastName));
        }

        public async Task<PersonServiceDto?> UpdatePersonAsync(int id, string firstName, string lastName)
        {
            var person = await personRepository.UpdatePersonAsync(id, firstName, lastName);
            if (person == null)
            {
                return null!;
            }
            return new PersonServiceDto(new Person(person.Id, person.FirstName, person.LastName));
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            return await personRepository.DeletePersonAsync(id);
        }
    }
}
