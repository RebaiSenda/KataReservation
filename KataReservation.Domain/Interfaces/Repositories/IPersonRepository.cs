using KataReservation.Domain.Dtos.Repositories;

namespace KataReservation.Domain.Interfaces.Repositories
{
    public interface IPersonRepository
    {
        Task<PersonRepositoryDto?> GetPersonByIdAsync(int id);
    }
}
