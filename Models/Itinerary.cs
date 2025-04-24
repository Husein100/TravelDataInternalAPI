using System.Text.Json.Serialization;

namespace TravelDataInternalAPI.Models
{
    public class Itinerary
    {
        [JsonPropertyName("segments")]
        public List<Segment> Segments { get; set; }
    }
}
