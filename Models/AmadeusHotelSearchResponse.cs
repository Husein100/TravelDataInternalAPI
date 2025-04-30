namespace TravelDataInternalAPI.Models
{

    public class AmadeusHotelSearchResponse
    {
        public List<HotelData> Data { get; set; }

        public class HotelData
        {
            public string Name { get; set; }
            public Address Address { get; set; }
            public RatePlan RatePlan { get; set; }
            public int? StarRating { get; set; }
            public List<Facility> Facilities { get; set; }
            public List<Picture> Pictures { get; set; }
        }

        public class Address
        {
            public string Line { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
        }

        public class RatePlan
        {
            public Price Price { get; set; }
        }

        public class Price
        {
            public decimal Total { get; set; }
        }

        public class Facility
        {
            public string Name { get; set; }
        }

        public class Picture
        {
            public string Uri { get; set; }
        }
    }
}
