using System.Text.Json.Serialization;

namespace TravelDataInternalAPI.Models
{

    public class Price
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("total")]
        public string Total { get; set; }
    }
}
