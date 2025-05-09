using Microsoft.OpenApi.Models;
using TravelDataInternalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Registrering af AmadeusAuthService og AmadeusFlightService i Program.cs
builder.Services.AddHttpClient<AmadeusAuthService>();
builder.Services.AddScoped<AmadeusFlightService>();
builder.Services.AddScoped<AmadeusAccommodationService>();

// Tilføj CORS-konfiguration for at tillade anmodninger fra forskellige domæner
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Tilføj Swagger/OpenAPI-tjenesterne
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});

var app = builder.Build();

// Konfigurer CORS
app.UseCors();

// Brug HTTPS-omdirigering for at sikre, at alle anmodninger bruger HTTPS
app.UseHttpsRedirection();

// Brug routing og autorisation
app.UseRouting();
app.UseAuthorization();

// Brug Swagger UI til at vise API-dokumentation
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Dette er den URL, der bruges til at tilgå din Swagger JSON
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    // RoutePrefix er sat til string.Empty for at gøre Swagger UI tilgængelig på roden af din API
    c.RoutePrefix = string.Empty;
});

// Map controllers, så de bliver tilgængelige som API-endpoints
app.MapControllers();

// Start applikationen
app.Run();
