using System.Text.Json.Serialization;
using Carly.Api.Features.Rental;
using Carly.Api.Features.Rental.Pricing;
using Carly.Api.Features.Rental.Register;
using Carly.Api.Features.Rental.Return;
using Carly.Api.Features.Shared.Models;
using Carly.Api.Infrastructure;
using RentalModel = Carly.Api.Features.Shared.Models.Rental;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Carly Rental API",
        Version = "v1",
        Description = "API for registering vehicle pickup and return in the Carly rental lifecycle."
    });
});
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(allowIntegerValues: false));
    });
builder.Services.AddSingleton<IRentalStore>(_ =>
{
    var store = new InMemoryRentalStore();
    store.Add(new RentalModel
    {
        BookingNumber = "BOOK-001",
        RegistrationNumber = "REG001",
        Category = CarCategory.SmallCar
    });
    store.Add(new RentalModel
    {
        BookingNumber = "BOOK-002",
        RegistrationNumber = "REG002",
        Category = CarCategory.Combi
    });
    store.Add(new RentalModel
    {
        BookingNumber = "BOOK-003",
        RegistrationNumber = "REG003",
        Category = CarCategory.Truck
    });
    return store;
});
builder.Services.AddScoped<RegisterVehicleService>();
builder.Services.AddScoped<ReturnVehicleService>();
builder.Services.AddSingleton(new PricingConfiguration(
    100m,
    2m));
builder.Services.AddSingleton<PricingService>();

var app = builder.Build();

app.UseSwagger();   
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();

public partial class Program;
