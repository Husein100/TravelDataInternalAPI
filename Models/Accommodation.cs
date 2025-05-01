namespace TravelDataInternalAPI.Models
{
    public class Accommodation
    {


        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string AccommodationName { get; set; }
        public decimal PricePerNight { get; set; }
        public int StarRating { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<string> Facilities { get; set; }
        public string AccommodationImageUrl { get; set; }
        public string AvailableRoomsStatus { get; set; }
        public int LengthOfStay { get; set; }
        public string RoomType { get; set; }
        public string RoomTypeDescription { get; set; }
    }

}