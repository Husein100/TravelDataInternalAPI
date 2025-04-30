using System.Net.Http.Headers;
using TravelDataInternalAPI.Models;

namespace TravelDataInternalAPI.Services
{
    public class AmadeusHotelService
    {
        private readonly HttpClient _httpClient;
        private readonly AmadeusAuthService _authService;
        private readonly IConfiguration _configuration;

        // Constructor to inject HttpClient, AmadeusAuthService, and IConfiguration
        public AmadeusHotelService(HttpClient httpClient, AmadeusAuthService authService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authService = authService;
            _configuration = configuration;
        }

        // Method to search for hotels
        public async Task<List<Accommodation>> SearchHotelsAsync(string city, DateTime checkInDate, DateTime checkOutDate)
        {
            Console.WriteLine($"Starting hotel search: City={city}, CheckIn={checkInDate:yyyy-MM-dd}, CheckOut={checkOutDate:yyyy-MM-dd}");

            // Dummy Access Token since we're not using a real API
            var accessToken = "dummy_access_token"; // Use a dummy token here
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Failed to obtain access token.");
                return new List<Accommodation>();
            }
            Console.WriteLine("Access token obtained successfully.");

            // Setup HttpClient Authorization (not necessary for dummy data, but kept for consistency)
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Instead of calling the actual Amadeus API, we use dummy data
            Console.WriteLine("Using dummy data for hotel search...");

            var dummyResponse = GetDummyHotelResponse(); // Simulate the Amadeus API response

            if (dummyResponse?.Data == null || !dummyResponse.Data.Any())
            {
                Console.WriteLine("No hotels found in dummy data.");
                return new List<Accommodation>();
            }

            // Map the dummy response data to the Accommodation model
            var accommodations = dummyResponse.Data.Select(hotel => new Accommodation
            {
                Country = hotel.Address?.Country ?? "Unknown",  // Default to "Unknown" if Country is null
                City = hotel.Address?.City ?? "Unknown",  // Default to "Unknown" if City is null
                Address = hotel.Address?.Line ?? "No address provided",  // Default address
                AccommodationName = hotel.Name ?? "No name available",  // Default name if not available
                PricePerNight = hotel.RatePlan?.Price?.Total ?? 0,  // Default price to 0 if not available
                StarRating = hotel.StarRating ?? 0,  // Default to 0 stars if not available
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                Facilities = hotel.Facilities?.Select(f => f.Name).ToList() ?? new List<string>(),  // Default to empty list if no facilities
                AccommodationImageUrl = hotel.Pictures?.FirstOrDefault()?.Uri ?? "default-image-url.jpg",  // Fallback image if none available
                AvailableRoomsStatus = "Available",  // Hardcoded for the dummy data, can be adjusted later
                LengthOfStay = (checkOutDate - checkInDate).Days,
                RoomType = "Standard",  // Adjust based on real data model
                RoomTypeDescription = "Standard room"  // Adjust based on real data model
            }).ToList();

            Console.WriteLine($"Found {accommodations.Count} hotels.");
            return accommodations;

        }

        // Method to generate dummy hotel data for testing
        private AmadeusHotelSearchResponse GetDummyHotelResponse()
        {
            return new AmadeusHotelSearchResponse
            {
                Data = new List<AmadeusHotelSearchResponse.HotelData>
                {
                    new AmadeusHotelSearchResponse.HotelData
                    {
                        Name = "Hotel XYZ",
                        Address = new AmadeusHotelSearchResponse.Address
                        {
                            Country = "USA",
                            City = "New York",
                            Line = "123 Example Street"
                        },
                        RatePlan = new AmadeusHotelSearchResponse.RatePlan
                        {
                            Price = new AmadeusHotelSearchResponse.Price
                            {
                                Total = 150
                            }
                        },
                        StarRating = 4,
                        Facilities = new List<AmadeusHotelSearchResponse.Facility>
                        {
                            new AmadeusHotelSearchResponse.Facility { Name = "Wi-Fi" },
                            new AmadeusHotelSearchResponse.Facility { Name = "Pool" }
                        },
                        Pictures = new List<AmadeusHotelSearchResponse.Picture>
                        {
                            new AmadeusHotelSearchResponse.Picture { Uri = "https://example.com/image.jpg" }
                        }
                    },
                    new AmadeusHotelSearchResponse.HotelData
                    {
                        Name = "Hotel ABC",
                        Address = new AmadeusHotelSearchResponse.Address
                        {
                            Country = "France",
                            City = "Paris",
                            Line = "456 Example Avenue"
                        },
                        RatePlan = new AmadeusHotelSearchResponse.RatePlan
                        {
                            Price = new AmadeusHotelSearchResponse.Price
                            {
                                Total = 200
                            }
                        },
                        StarRating = 5,
                        Facilities = new List<AmadeusHotelSearchResponse.Facility>
                        {
                            new AmadeusHotelSearchResponse.Facility { Name = "Gym" },
                            new AmadeusHotelSearchResponse.Facility { Name = "Restaurant" }
                        },
                        Pictures = new List<AmadeusHotelSearchResponse.Picture>
                        {
                            new AmadeusHotelSearchResponse.Picture { Uri = "https://example.com/another_image.jpg" }
                        }
                    }
                }
            };
        }
    }
}
