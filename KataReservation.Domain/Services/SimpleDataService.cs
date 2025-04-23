// KataReservation.Domain/Services/SimpleDataService.cs
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Refit;

namespace KataReservation.Domain.Services
{
    public class SimpleDataService : ISimpleDataService
    {
        private readonly IKataSimpleApiClient _apiClient;
        private readonly ILogger<SimpleDataService> _logger;

        public SimpleDataService(IKataSimpleApiClient apiClient, ILogger<SimpleDataService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<SimpleDataResponse> GetExternalDataAsync()
        {
            try
            {
                _logger.LogInformation("Récupération des données externes via KataSimpleAPI");
                var result = await _apiClient.GetDataAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des données externes");
                throw new Exception("Une erreur est survenue lors de la récupération des données externes", ex);
            }
        }
    }
}