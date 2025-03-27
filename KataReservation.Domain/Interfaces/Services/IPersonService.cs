using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Domain.Interfaces.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonServiceDto>> GetPersonsAsync();
        Task<PersonServiceDto?> GetPersonByIdAsync(int id);
        Task<PersonServiceDto> CreatePersonAsync(string firstName, string lastName);
        Task<PersonServiceDto?> UpdatePersonAsync(int id, string firstName, string lastName);
        Task<bool> DeletePersonAsync(int id);
    }
}
