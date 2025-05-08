using TravelDataInternalAPI.Models;

namespace Gotorz2.Client.Models
{

    public class TravelPackage
    {
        public string TripTitle { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public RoundTripFlight Flight { get; set; }
        public Accommodation Hotel { get; set; }
        public string Description { get; set; }
        public decimal TripTotalPrice { get; set; }

    }


}
