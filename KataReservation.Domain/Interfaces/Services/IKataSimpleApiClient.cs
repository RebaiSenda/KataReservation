using Refit;
using KataReservation.Domain.Models;

namespace KataReservation.Domain.Interfaces.Services
{
    public interface IKataSimpleApiClient
    {
        [Get("/api/data")]
        Task<SimpleDataResponse> GetDataAsync();
    }
}