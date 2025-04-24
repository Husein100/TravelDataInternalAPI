using System.Text.Json.Serialization;

namespace TravelDataInternalAPI.Models
{
    public class Segment
    {
        [JsonPropertyName("departure")]
        public FlightPoint Departure { get; set; }

        [JsonPropertyName("arrival")]
        public FlightPoint Arrival { get; set; }

        [JsonPropertyName("carrierCode")]
        public string CarrierCode { get; set; }

        [JsonPropertyName("duration")]
        public string Duration { get; set; }
    }
}
