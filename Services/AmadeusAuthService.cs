using System.Text.Json;

//Opgave klassen har er, at sørger for, at  applikationen automatisk henter en access token fra Amadeus' API, så vi kan sende autoriserede forespørgsler til deres tjenester.
//Hvad laver klassen?
//1. Den opretter en HTTP-klient, som bruges til at sende anmodninger til Amadeus' API.
//2. Den henter konfigurationen fra appsettings.json, som indeholder oplysninger som ClientId, ClientSecret og TokenUrl.
//3. Den sender en POST-anmodning til Amadeus' token-URL med de nødvendige legitimationsoplysninger (ClientId og ClientSecret) for at få en adgangstoken.Ved at sende ClientId og ClientSecret til Amadeus, får vi en token, som repræsenterer vores identitet.
//4. Den modtager og returnerer adgangstokenet, som kan bruges til at autentificere fremtidige anmodninger til Amadeus' API.
//Når vi har tokenen, kan vi bruge den som adgangskort til fx Flight Search, Hotel Offers, m.m.Tokenen er midlertidig og bruges i stedet for at sende vores følsomme oplysninger hver gang.
//5. Hvis anmodningen mislykkes, returnerer den null.
//6. Den bruger FormUrlEncodedContent til at sende dataene i det rigtige format, som Amadeus' API forventer.
//7. Den håndterer JSON-responsen fra Amadeus' API og udtrækker adgangstokenet fra den.
//8. Den bruger JsonSerializer til at deserialisere JSON-responsen til en ordbog, så vi kan få adgang til tokenet.
//9. Den bruger async/await for at sikre, at anmodningen ikke blokerer hovedtråden, hvilket gør applikationen mere responsiv.
//10. Den bruger IConfiguration til at hente konfigurationen, hvilket gør det nemt at ændre indstillingerne uden at ændre koden.
//11. Den bruger HttpClient til at sende HTTP-anmodninger, hvilket er en standardmetode i .NET til at kommunikere med web-API'er.
//12. Den bruger Dictionary til at oprette en samling af nøgle-værdi-par, der repræsenterer de data, der skal sendes i anmodningen.
//13.
namespace TravelDataInternalAPI.Services
{
    //Hvorfor bruge en token her? Du skal bruge tokenen for at: Bevise hvem vi er, vi får adgang til API’erne
    //, vi beskytte vores credentials
    public class AmadeusAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AmadeusAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// Metode til at hente adgangstoken fra Amadeus API
        public async Task<string?> GetAccessTokenAsync()
        {
            var clientId = _configuration["Amadeus:ClientId"];
            var clientSecret = _configuration["Amadeus:ClientSecret"];
            var tokenUrl = _configuration["Amadeus:TokenUrl"];

            var requestData = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };

            var requestContent = new FormUrlEncodedContent(requestData);

            var response = await _httpClient.PostAsync(tokenUrl, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                // Log eller kast exception efter behov
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (data != null && data.TryGetValue("access_token", out var token))
            {
                return token?.ToString();
            }

            return null;
        }
    }

}
