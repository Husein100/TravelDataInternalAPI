using Newtonsoft.Json;
using System.Net.Http.Headers;
using TravelDataInternalAPI.Models;

//Serviceklassen står for selve arbejdet: logik, datahåndtering og kommunikation med fx databaser eller eksterne systemer som Amadeus.
//Den indeholder metoder, der udfører specifikke opgaver, som controlleren kan kalde.
//Serviceklassen er ansvarlig for at håndtere forretningslogik og dataadgang.
//Den fungerer som bindeled mellem controlleren og datakilderne (fx databaser eller eksterne API'er).
//Den håndterer også eventuelle fejl og undtagelser, der måtte opstå under behandlingen af anmodningerne.
//Den er ansvarlig for at hente data fra Amadeus API'et og returnere det til controlleren.
//Den ved hvordan man "laver retten" – f.eks. hvordan man spørger Amadeus efter flyrejser.

namespace TravelDataInternalAPI.Services
{

    public class AmadeusFlightService
    {
        // Denne klasse håndterer kommunikationen med Amadeus API'et for at søge efter flyrejser.
        // Den bruger HttpClient til at sende HTTP-anmodninger og modtage svar.
        // Den bruger også AmadeusAuthService til at hente adgangstoken, som er nødvendig for at autentificere anmodningerne.
        // Den bruger IConfiguration til at hente konfigurationen fra appsettings.json, som indeholder oplysninger som ClientId, ClientSecret og FlightOffersUrl.

        private readonly HttpClient _httpClient;
        private readonly AmadeusAuthService _authService;
        private readonly IConfiguration _configuration;

        // Konstruktøren tager en HttpClient, AmadeusAuthService og IConfiguration som parametre.
        // Den initialiserer de private felter med de modtagne værdier.
        // HttpClient bruges til at sende HTTP-anmodninger.
        // AmadeusAuthService bruges til at hente adgangstoken.
        // IConfiguration bruges til at hente konfigurationen fra appsettings.json.
        // Konstruktøren gør det muligt at injicere afhængighederne, hvilket gør klassen lettere at teste og vedligeholde.


        // Det her er en såkaldt "konstruktør" – det er en særlig funktion, der bliver kørt, når man laver en ny udgave (instans) af AmadeusFlightService.
        // Konstruktøren tager tre ting med sig (kaldet "parametre"), som den får udefra: httpClient, authService og configuration.
        public AmadeusFlightService(HttpClient httpClient, AmadeusAuthService authService, IConfiguration configuration)
        {
            // Her gemmer vi den modtagne httpClient i en privat variabel kaldet _httpClient, så vi kan bruge den senere i klassen.
            // httpClient bruges til at sende forespørgsler ud på internettet – fx hente data fra en flydatabase.
            _httpClient = httpClient;

            // Her gemmer vi authService i en privat variabel kaldet _authService.
            // authService bruges til at håndtere adgangstilladelser, altså sørge for at vi har "nøglen" til at hente data fra Amadeus.
            _authService = authService;

            // Her gemmer vi configuration i en privat variabel kaldet _configuration.
            // configuration bruges til at læse indstillinger og opsætninger (fx API-nøgler eller links) fra en fil eller et system.
            _configuration = configuration;
        }

        // Denne metode søger efter flyrejser baseret på oprindelse, destination og afrejsedato.
        // Den tager også imod en returdato og antallet af voksne, børn og spædbørn.
        public async Task<List<RoundTripFlight>> SearchFlightsAsync(string origin, string destination, string departureDate, string? returnDate = null, int adults = 1, int children = 0, int infants = 0)
        {
            // Her kalder vi AmadeusAuthService for at få en adgangstoken, som er nødvendig for at kunne kommunikere med Amadeus API'et.
            // Adgangstokenet er som en nøgle, der giver os adgang til at hente data fra Amadeus.
            // Vi kalder GetAccessTokenAsync metoden, som returnerer en token.
            // Hvis tokenet er null eller tomt, returnerer vi null, hvilket betyder at vi ikke kan hente flydata.
            // Det er vigtigt at have en gyldig token, ellers kan vi ikke få adgang til API'et.
            var accessToken = await _authService.GetAccessTokenAsync();

            // Hvis tokenet er ugyldigt, logger vi og returnerer null
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Adgangstoken er ugyldig eller tom.");
                return null;
            }

            // Her sætter vi HTTP-headeren "Authorization" på vores httpClient til at bruge den adgangstoken, vi lige har fået.
            // Det betyder at vi fortæller Amadeus API'et, hvem vi er, og at vi har ret til at hente data.
            // Authorization headeren er vigtig, fordi den fortæller API'et, at vi har de nødvendige tilladelser til at få adgang til de data, vi beder om.

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Her bygger vi URL'en til Amadeus API'et, hvor vi skal sende vores forespørgsel.
            // Vi henter URL'en fra konfigurationen (appsettings.json), så vi ikke skal skrive den direkte i koden.
            // URL'en indeholder oplysninger om, hvor vi vil flyve fra (origin), hvor vi vil flyve hen (destination), hvornår vi vil rejse (departureDate),
            // og eventuelt hvornår vi vil rejse tilbage (returnDate).

            var url = _configuration["Amadeus:FlightOffersUrl"];
            var requestUrl = $"{url}?originLocationCode={origin}" +
                             $"&destinationLocationCode={destination}" +
                             $"&departureDate={departureDate}" +
                             $"{(returnDate != null ? $"&returnDate={returnDate}" : "")}" +
                             $"&adults={adults}&children={children}&infants={infants}" +
                             $"&nonStop=true&max=20";

            // Her sender vi en GET-anmodning til Amadeus API'et med den URL, vi lige har bygget.
            // Vi venter på svaret fra API'et, og gemmer det i en variabel kaldet "response".
            // response indeholder oplysninger om, hvorvidt anmodningen var succesfuld eller ej.
            // Hvis anmodningen ikke var succesfuld (f.eks. hvis der ikke blev fundet nogen flyrejser), returnerer vi null.
            // Det er vigtigt at tjekke, om anmodningen var succesfuld, så vi ikke prøver at arbejde med data, der ikke findes.

            var response = await _httpClient.GetAsync(requestUrl);

            // Her tjekker vi, om svaret fra API'et var succesfuldt (statuskode 200).
            // Hvis det ikke var, returnerer vi null, hvilket betyder at vi ikke kan hente flydata.
            // Det er vigtigt at tjekke statuskoden, så vi kan håndtere fejl korrekt og ikke forsøge at arbejde med ugyldige data.
            // Hvis svaret ikke var succesfuldt, returnerer vi null.
            // Hvis statuskode ikke er 200 OK, logger vi fejlen og returnerer null
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fejl ved API-kald. Statuskode: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Fejlindhold fra API: " + errorContent);
                return null;
            }

            // Her læser vi indholdet af svaret fra API'et som en streng (JSON-format).
            // Vi bruger ReadAsStringAsync metoden til at hente indholdet, og gemmer det i en variabel kaldet "json".
            // JSON-formatet er et standardformat til at udveksle data mellem servere og klienter.
            // Det er vigtigt at læse indholdet som en streng, så vi kan deserialisere det til vores egne objekter senere.

            var json = await response.Content.ReadAsStringAsync();

            // Her deserialiserer vi JSON-strengen til et FlightSearchResponse objekt ved hjælp af JsonConvert.
            // Deserialisering betyder at vi konverterer JSON-strengen til et objekt, så vi kan arbejde med det i vores kode.
            // Vi bruger JsonConvert.DeserializeObject metoden til at gøre dette.
            // FlightSearchResponse er en klasse, der repræsenterer strukturen af svaret fra Amadeus API'et.
            // Det er vigtigt at deserialisere JSON-strengen, så vi kan få adgang til de data, vi har brug for.

            // Deserialiserer JSON til FlightSearchResponse objekt
            FlightSearchResponse flightSearchResponse;
            try
            {
                flightSearchResponse = JsonConvert.DeserializeObject<FlightSearchResponse>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fejl ved deserialisering af JSON: " + ex.Message);
                return null;
            }


            // Her tjekker vi, om flightSearchResponse er null eller om der ikke er nogen flytilbud i svaret.
            // Hvis det er tilfældet, returnerer vi null, hvilket betyder at vi ikke kan hente flydata.
            // Det er vigtigt at tjekke, om flightSearchResponse er null, så vi ikke forsøger at arbejde med data, der ikke findes.
            // Hvis flightSearchResponse er null eller der ikke er nogen flytilbud, returnerer vi null.
            // Det er vigtigt at tjekke, om der er flytilbud, så vi kan håndtere situationen korrekt og ikke forsøge at arbejde med ugyldige data.
            var offers = flightSearchResponse?.Data;
            // Her tjekker vi, om offer er null eller om der ikke er mindst to itineraries (ruter) i tilbuddet.

            // Hvis der ikke er nogen flytilbud, logger vi og returnerer en tom liste
            if (offers == null || !offers.Any())
            {
                Console.WriteLine("Ingen flytilbud fundet.");
                return new List<RoundTripFlight>();
            }

            var roundTrips = new List<RoundTripFlight>();

            foreach (var offer in offers)
            {
                // Hvis der ikke er både ud- og hjemrejse, springes tilbuddet over
                if (offer.Itineraries.Count < 2)
                {
                    Console.WriteLine("Tilbud droppet – indeholder ikke både ud- og hjemrejse.");
                    continue;
                }

                // Her mapper vi de første to itineraries (ruter) i tilbuddet til FlightInfo objekter.
                // Vi bruger MapToFlightInfo metoden til at gøre dette.
                // Den første itinerary repræsenterer udrejse (outbound), og den anden repræsenterer hjemrejse (inbound).
                // Vi gemmer de mapperede objekter i variablerne outboundFlight og inboundFlight.
                // Vi bruger MapToFlightInfo metoden til at konvertere itineraries til FlightInfo objekter.
                // MapToFlightInfo metoden tager en itinerary og den samlede pris som parametre.
                // Den mapper segmenterne i itineraryen til FlightInfo objektet, som indeholder oplysninger om afgang, ankomst, varighed og pris.

                var outboundFlight = MapToFlightInfo(offer.Itineraries[0], offer.Price.Total);
                var inboundFlight = MapToFlightInfo(offer.Itineraries[1], offer.Price.Total);

                // Her opretter vi et RoundTripFlight objekt, som indeholder oplysninger om udrejse og hjemrejse.
                // Vi bruger RoundTripFlight klassen til at repræsentere en rundrejse med udrejse og hjemrejse.
                // Vi sætter outbound og inbound egenskaberne til de mapperede FlightInfo objekter.
                // Vi sætter også TotalPrice egenskaben til den samlede pris for flyvningen.
                // TotalPrice er en streng, der indeholder prisen og valutaen (f.eks. "1000 USD").
                // Vi bruger offer.Price.Total og offer.Price.Currency til at få prisen og valutaen fra tilbuddet.
                // Til sidst returnerer vi RoundTripFlight objektet, som indeholder oplysningerne om udrejse og hjemrejse.

                roundTrips.Add(new RoundTripFlight
                {
                    Outbound = outboundFlight,
                    Inbound = inboundFlight,
                    TotalPrice = offer.Price.Total + " " + offer.Price.Currency
                });
            }

            return roundTrips; // 🔥 Den manglede!
        }



        // Denne metode mapper en itinerary (rute) til et FlightInfo objekt.
        // Den tager en itinerary og den samlede pris som parametre.
        // Den mapper segmenterne i itineraryen til FlightInfo objektet, som indeholder oplysninger om afgang, ankomst, varighed og pris.
        // Den returnerer et FlightInfo objekt, som indeholder oplysningerne om flyvningen.
        private FlightInfo MapToFlightInfo(Itinerary itinerary, string totalPrice)
        {
            // Her mapper vi den første segment i itineraryen til et FlightInfo objekt.
            // Vi bruger itinerary.Segments.First() til at få det første segment i ruten.
            // Segmentet indeholder oplysninger om afgang, ankomst, varighed og flyselskab.
            // Vi bruger First() metoden til at få det første element i listen af segmenter.
            // Det første segment repræsenterer udrejse (outbound) og det andet repræsenterer hjemrejse (inbound).

            var segment = itinerary.Segments.First();

            // Her opretter vi et FlightInfo objekt, som indeholder oplysninger om flyvningen.
            // Vi bruger FlightInfo klassen til at repræsentere en flyvning med afgang, ankomst, varighed og pris.
            // Vi sætter DepartureAirport egenskaben til afgangslufthavnen (IATA-koden) fra segmentet.
            // Vi sætter ArrivalAirport egenskaben til ankomstlufthavnen (IATA-koden) fra segmentet.
            // Vi sætter DepartureTime egenskaben til afgangstidspunktet fra segmentet.
            // Vi sætter ArrivalTime egenskaben til ankomsttidspunktet fra segmentet.
            // Vi sætter Duration egenskaben til varigheden af flyvningen fra segmentet.
            // Vi sætter Price egenskaben til den samlede pris, som vi fik fra tilbuddet.

            return new FlightInfo
            {
                DepartureAirport = segment.Departure.IataCode,
                ArrivalAirport = segment.Arrival.IataCode,
                DepartureTime = segment.Departure.At,
                ArrivalTime = segment.Arrival.At,
                Duration = segment.Duration,
                Price = totalPrice
            };
        }

    }
}

