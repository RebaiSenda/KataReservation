using KataReservation.Domain.Models;

namespace KataReservation.Domain.Interfaces.Services
{
    public interface ISimpleDataService
    {
        Task<SimpleDataResponse> GetExternalDataAsync();
    }
}
