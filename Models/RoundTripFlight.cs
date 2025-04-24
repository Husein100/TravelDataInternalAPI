namespace TravelDataInternalAPI.Models
{
    public class RoundTripFlight
    {
        public FlightInfo Outbound { get; set; }
        public FlightInfo Inbound { get; set; }
        public string TotalPrice { get; set; }
    }


}
