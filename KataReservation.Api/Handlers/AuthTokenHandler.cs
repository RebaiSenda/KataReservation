using Duende.IdentityModel.Client;

namespace KataReservation.Api.Handlers
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private string? _cachedToken;
        private DateTime _tokenExpiration = DateTime.MinValue;

        public AuthTokenHandler(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            //InnerHandler = new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_cachedToken == null || DateTime.UtcNow >= _tokenExpiration)
            {
                await GetTokenAsync();
            }

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachedToken);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task GetTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var discoveryEndpoint = _configuration["IdentityServer:Authority"] ?? "https://localhost:5001";
            var disco = await client.GetDiscoveryDocumentAsync(discoveryEndpoint);

            if (disco.IsError)
            {
                throw new Exception($"Erreur lors de la découverte du serveur d'identité: {disco.Error}");
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _configuration["IdentityServer:ClientId"] ?? "katasimpleAPI",
                ClientSecret = _configuration["IdentityServer:ClientSecret"] ?? "secret",
                Scope = "KataSimpleAPI"
            });

            if (tokenResponse.IsError)
            {
                throw new Exception($"Erreur lors de l'obtention du token: {tokenResponse.Error}");
            }

            _cachedToken = tokenResponse.AccessToken;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // Garde une marge de 60 secondes
        }
    }
}
