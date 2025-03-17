using KataReservation.Domain.Dtos.Repositories;

namespace KataReservation.Domain.Interfaces.Repositories
{
    public interface IPersonRepository
    {
        Task<IEnumerable<PersonRepositoryDto>> GetPersonsAsync();
        Task<PersonRepositoryDto> GetPersonByIdAsync(int id);
        Task<PersonRepositoryDto> CreatePersonAsync(string firstName, string lastName);
        Task<PersonRepositoryDto> UpdatePersonAsync(int id, string firstName, string lastName);
        Task<bool> DeletePersonAsync(int id);
    }
}
