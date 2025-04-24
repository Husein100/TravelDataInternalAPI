using System.Text.Json.Serialization;

namespace TravelDataInternalAPI.Models
{
    public class FlightOffer
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("itineraries")]
        public List<Itinerary> Itineraries { get; set; }

        [JsonPropertyName("price")]
        public Price Price { get; set; }
    }
}
