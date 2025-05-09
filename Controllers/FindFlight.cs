using Microsoft.AspNetCore.Mvc;
using TravelDataInternalAPI.Models;
using TravelDataInternalAPI.Services;

//Trin 2 kommer efter trin 1 som er at oprette en model: Opret en controller, der håndterer HTTP-anmodninger
// Controlleren styrer, hvad der skal ske, når nogen sender en HTTP-anmodning (som f.eks. når en bruger eller en app spørger om data eller sender data til serveren).
//En controller styrer HTTP-anmodninger ved at modtage HTTP anmodninger, udføre logik (f.eks. hente eller gemme data) og sende et svar (HTTP-response) tilbage (typisk i JSON-format)
// til den, der sendte anmodningen.

//Controlleren er ansvarlig for at håndtere anmodninger fra klienter (f.eks. webbrowser, mobilapp) og returnere de relevante data eller handlinger.
//Controlleren fungerer som en mellemmand mellem klienten og serveren, og den håndterer logikken for at finde og returnere de ønskede data.
//Controllerens opgave?Den tager imod bestillinger fra gæsterne (brugerne eller systemer via API-kald).
//Den finder de ønskede flyrejser ved at spørge Amadeus API'et og returnerer dem til gæsten.
//Den beder service-klassen om at lave arbejdet.Til sidst serverer controlleren resultatet tilbage til brugeren.

//Hvordan arbejder de sammen?Brugeren sender en forespørgsel (fx via app eller browser).
//Controlleren modtager forespørgslen og sender den videre til service-klassen.
//Service-klassen henter data fra Amadeus API'et og returnerer dem til controlleren.
//Controlleren sender svaret tilbage til brugeren i et format, de kan forstå (fx JSON).
//Klar ansvarsfordeling: Controller = input/output, Service = logik.


[ApiController]  // Angiver, at dette er en API-controller, der er designet til at håndtere HTTP-anmodninger.
[Route("api/flights")]  // Definerer ruten for controlleren. Ruten bliver "", da controllerens navn er FindFlightsController.

//Controlleren hedder FindFlightsController og den håndterer flysøgninger via en HTTP GET-anmodning.
public class FindFlightsController : ControllerBase
{
    // Deklarerer en privat readonly variabel, der refererer til AmadeusFlightService, som håndterer kommunikationen med den eksterne Amadeus API.

    private readonly AmadeusFlightService _flightService;

    // Konstruktør, der tager en instans af AmadeusFlightService som parameter og initialiserer _flightService.
    public FindFlightsController(AmadeusFlightService flightService)
    {
        _flightService = flightService;
    }

    // Denne GET-metode håndterer forespørgsler til /api/findflights/search, som giver brugeren mulighed for at søge efter flyrejser.
    // Den forventer, at parametrene origin (afrejseby), destination (destinationby) og date (afrejsedato) bliver sendt som query parameters.
    // Min controller – specifikt metoden SearchFlights() – returnerer et JSON-objekt af typen RoundTripFlight, som indeholder en liste over flyrejser.
    //Controlleren forventer og returnerer samme type som serviceklassen giver, som er RoundTripFlight
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RoundTripFlight>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<RoundTripFlight>>> SearchFlights(
        [FromQuery] string origin,        // Oprindelsesbyen (fx CPH for København)
        [FromQuery] string destination,   // Destinationsbyen (fx BER for Berlin)
        [FromQuery] string date,          // Afrejsedato (fx 2025-04-25)
        [FromQuery] string? returnDate)   // Returdato (fx 2025-05-02)
    {
        //  Sørg for at brugeren har angivet en retur-dato – ellers afvis forespørgslen.
        if (string.IsNullOrEmpty(returnDate))
        {
            return BadRequest("Du skal angive en returdato for at søge efter rundrejser.");
        }

        // Kald servicen med alle parametre inkl. returnDate
        var results = await _flightService.SearchFlightsAsync(origin, destination, date, returnDate);

        // Hvis ingen resultater, send 404
        if (results == null || !results.Any())
        {
            return NotFound("Ingen flyrejser blev fundet.");
        }

        // Ellers send resultaterne tilbage
        return Ok(results);
    }
}

