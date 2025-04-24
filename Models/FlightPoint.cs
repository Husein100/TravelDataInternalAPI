using System.Text.Json.Serialization;

namespace TravelDataInternalAPI.Models
{
    public class FlightPoint
    {
        [JsonPropertyName("iataCode")]
        public string IataCode { get; set; }

        [JsonPropertyName("at")]
        public string At { get; set; }
    }
}
