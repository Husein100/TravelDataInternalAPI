using System.Net.Http.Headers;
using TravelDataInternalAPI.Models;

namespace TravelDataInternalAPI.Services
{
    // Serviceklasse der håndterer hoteldata via Amadeus (her med dummydata)
    public class AmadeusAccommodationService
    {
        private readonly HttpClient _httpClient; // Til HTTP-kald
        private readonly AmadeusAuthService _authService; // Til fremtidig brug af autentificering (ikke i brug her)
        private readonly IConfiguration _configuration; // Til konfiguration (ikke brugt i dummy-versionen)

        // Dependency Injection af HttpClient, AmadeusAuthService og IConfiguration
        public AmadeusAccommodationService(HttpClient httpClient, AmadeusAuthService authService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authService = authService;
            _configuration = configuration;
        }

        // Metode til at søge hoteller baseret på by og datoer (brug af dummydata)
        public async Task<List<Accommodation>> SearchHotelsAsync(string city, DateTime checkInDate, DateTime checkOutDate)
        {
            // Udskriv information om søgningen
            Console.WriteLine($"Starting hotel search: City={city}, CheckIn={checkInDate:yyyy-MM-dd}, CheckOut={checkOutDate:yyyy-MM-dd}");

            // Brug af en "dummy" access token, da ingen rigtig API bruges
            var accessToken = "dummy_access_token";

            // Check om token eksisterer (her altid sand)
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Failed to obtain access token.");
                return new List<Accommodation>(); // Returnér tom liste ved fejl
            }

            Console.WriteLine("Access token obtained successfully.");

            // Tilføj authorization-header til HTTP-klienten (selvom vi ikke bruger det reelt)
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            Console.WriteLine("Using dummy data for hotel search...");

            // Få dummyrespons (simuleret data i stedet for kald til Amadeus API)
            var dummyResponse = GetDummyHotelResponse();

            // Tjek om der er nogen hoteller i responsen
            if (dummyResponse?.Data == null || !dummyResponse.Data.Any())
            {
                Console.WriteLine("No hotels found in dummy data.");
                return new List<Accommodation>();
            }

            // Kortlæg dummy-data til vores domænemodel "Accommodation"
            var accommodations = dummyResponse.Data.Select(hotel => new Accommodation
            {
                Country = hotel.Address?.Country ?? "Unknown", // Brug fallback hvis felt mangler
                City = hotel.Address?.City ?? "Unknown",
                Address = hotel.Address?.Line ?? "No address provided",
                AccommodationName = hotel.Name ?? "No name available",
                PricePerNight = hotel.RatePlan?.Price?.Total ?? 0,
                StarRating = hotel.StarRating ?? 0,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                PictureUrl = hotel.Pictures?.FirstOrDefault()?.Uri ?? "default-image-url.jpg",
                Facilities = hotel.Facilities?.Select(f => f.Name).ToList() ?? new List<string>(),
                AccommodationImageUrl = hotel.Pictures?.FirstOrDefault()?.Uri ?? "default-image-url.jpg",
                AvailableRoomsStatus = "Available", // Hardcoded værdi for dummy
                LengthOfStay = (checkOutDate - checkInDate).Days,
                RoomType = "Standard", // Placeholder
                RoomTypeDescription = "Standard room" // Placeholder
            }).ToList();

            Console.WriteLine($"Found {accommodations.Count} hotels.");
            return accommodations; // Returnér listen over hoteller
        }

        // Privat metode der returnerer dummydata til tests og udvikling
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
                            new AmadeusHotelSearchResponse.Picture { Uri = "https://imgservice.casai.com/500x245/moonlight-hotel-bar-resto-ht-hinche-bc-11653999-0.jpg" }
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
                            new AmadeusHotelSearchResponse.Picture { Uri = "https://content.skyscnr.com/available/1395248305/1395248305_WxH.jpg" }
                        }
                    }
                }
            };
        }
    }
}
