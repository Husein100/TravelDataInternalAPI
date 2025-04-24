namespace TravelDataInternalAPI.Models
{
    public class FlightInfo
    {
        public string Id { get; set; }               // Unikt ID til at identificere flyet
        public string DepartureAirport { get; set; } // Fx "CPH" (Afgangslufthavn)
        public string ArrivalAirport { get; set; }   // Fx "BER" (Ankomstlufthavn)
        public string DepartureTime { get; set; }    // Fx "2025-04-25T10:00"
        public string ArrivalTime { get; set; }      // Fx "2025-04-25T12:00"
        public string Duration { get; set; }         // Fx "PT2H"
        public string Price { get; set; }            // Fx "199.50 EUR"
    }

}
