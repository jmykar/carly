using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Carly.Api.Tests.Contract;

public sealed class ApiContractTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiContractTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment("Development")).CreateClient();
    }

    [Fact]
    public async Task SwaggerDocumentDescribesVersionedRentalApi()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.StartsWith("3.0", root.GetProperty("openapi").GetString());
        Assert.Equal("Carly Rental API", root.GetProperty("info").GetProperty("title").GetString());
        var paths = root.GetProperty("paths");
        Assert.True(paths.TryGetProperty("/api/v1/rental/{bookingNumber}", out var pickup));
        Assert.True(paths.TryGetProperty("/api/v1/rental/{bookingNumber}/return", out var rentalReturn));
        Assert.Equal("Register vehicle pickup", pickup.GetProperty("post").GetProperty("summary").GetString());
        Assert.True(rentalReturn.TryGetProperty("post", out _));

        var schemas = root.GetProperty("components").GetProperty("schemas");
        var registerRequest = schemas.GetProperty("RegisterRequest");
        Assert.Contains("registrationNumber", RequiredProperties(registerRequest));
        Assert.Contains("customerId", RequiredProperties(registerRequest));
        Assert.Equal(0, registerRequest.GetProperty("properties").GetProperty("odometerKm").GetProperty("minimum").GetInt32());
        Assert.Equal(new[] { "SmallCar", "Combi", "Truck" }, schemas.GetProperty("CarCategory").GetProperty("enum").EnumerateArray().Select(value => value.GetString()));
        Assert.True(schemas.TryGetProperty("ProblemDetails", out _));
    }

    private static IEnumerable<string> RequiredProperties(JsonElement schema)
    {
        return schema.GetProperty("required").EnumerateArray().Select(value => value.GetString()!);
    }

    [Fact]
    public async Task HealthEndpointRemainsAvailable()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task InvalidPickupContractReturnsValidationProblemWithoutProcessing()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/rental/BOOK-001")
        {
            Content = JsonContent.Create(new { RegistrationNumber = "ABC123", CustomerId = "customer", Category = "SmallCar", PickupDateTime = "", OdometerKm = -1 })
        };
        request.Headers.Accept.ParseAdd("application/problem+json");
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("errors", out _));

        var validPickup = await _client.PostAsJsonAsync(
            "/api/v1/rental/BOOK-001",
            new { RegistrationNumber = "ABC123", CustomerId = "customer", Category = "SmallCar", PickupDateTime = new DateTime(2026, 9, 13, 10, 0, 0), OdometerKm = 1 });
        Assert.Equal(HttpStatusCode.OK, validPickup.StatusCode);

        var numericCategory = await _client.PostAsJsonAsync(
            "/api/v1/rental/BOOK-001",
            new { RegistrationNumber = "ABC123", CustomerId = "customer", Category = 0, PickupDateTime = new DateTime(2026, 9, 13, 11, 0, 0), OdometerKm = 2 });
        Assert.Equal(HttpStatusCode.BadRequest, numericCategory.StatusCode);
    }
}
