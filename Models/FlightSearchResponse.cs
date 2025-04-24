// Vi bruger System.Text.Json.Serialization for at kunne navngive JSON-felterne korrekt,
// så de matcher strukturen i svaret fra Amadeus API'et
using System.Text.Json.Serialization;

namespace TravelDataInternalAPI.Models
{
    // Denne klasse repræsenterer hele JSON-svaret fra Amadeus API'et,
    // og indeholder en liste af flytilbud ("data" i JSON).
    public class FlightSearchResponse
    {
        // Her binder vi JSON-feltet "data" til vores .NET-liste af FlightOffer-objekter
        [JsonPropertyName("data")]
        public List<FlightOffer> Data { get; set; } = new(); // Initialiserer listen, så den ikke er null
    }


}
