using Microsoft.AspNetCore.Mvc;
using TravelDataInternalAPI.Models;
using TravelDataInternalAPI.Services;

namespace TravelDataInternalAPI.Controllers
{
    // Controlleren er nu omdøbt til FindHotelsController, da vi arbejder med hoteller
    [ApiController]
    [Route("api/hotels")]  // Definerer ruten for hotel-relaterede anmodninger

    // FindHotelsController håndterer hotelforespørgsler via HTTP GET-anmodninger
    public class FindAccommodationController : ControllerBase
    {
        // Deklarerer en privat readonly variabel, der refererer til AmadeusAccommodationService, som håndterer kommunikationen med hotel-API'et
        private readonly AmadeusAccommodationService _hotelService;

        // Konstruktør, der tager en instans af AmadeusAccommodationService som parameter og initialiserer _hotelService.
        public FindAccommodationController(AmadeusAccommodationService hotelService)
        {
            _hotelService = hotelService;
        }

        // Denne GET-metode håndterer forespørgsler til /api/hotels/search, som giver brugeren mulighed for at søge efter hoteller.
        // Den forventer, at parametrene city (by), checkInDate (check-ind dato) og checkOutDate (check-ud dato) bliver sendt som query parameters.
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Accommodation>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Accommodation>>> SearchHotels(
            [FromQuery] string city,               // Byen (fx København)
            [FromQuery] DateTime checkInDate,      // Check-ind dato (fx 2025-04-25)
            [FromQuery] DateTime checkOutDate)     // Check-ud dato (fx 2025-05-02)
        {
            // Sørg for at brugeren har angivet både check-in og check-out datoer – ellers afvis forespørgslen.
            if (checkInDate == default || checkOutDate == default)
            {
                return BadRequest("Både check-in og check-out datoer skal angives for at søge efter hoteller.");
            }

            // Kald servicen med alle parametre
            var results = await _hotelService.SearchHotelsAsync(city, checkInDate, checkOutDate);

            // Hvis ingen resultater, send 404
            if (results == null || !results.Any())
            {
                return NotFound("Ingen hoteller blev fundet.");
            }

            // Ellers send resultaterne tilbage
            return Ok(results);
        }
    }
}