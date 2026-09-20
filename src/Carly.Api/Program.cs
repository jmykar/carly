using System.Text.Json.Serialization;
using Carly.Api.Features.Rental;
using Carly.Api.Features.Rental.Pricing;
using Carly.Api.Features.Rental.Register;
using Carly.Api.Features.Rental.Return;
using Carly.Api.Infrastructure;
using Carly.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

var connectionString = builder.Configuration.GetConnectionString("Carly")
                       ?? "Data Source=carly.db";
builder.Services.AddDbContext<CarlyDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IRentalStore, EfRentalStore>();
builder.Services.AddScoped<RegisterRentalService>();
builder.Services.AddScoped<ReturnRentalService>();
builder.Services.AddSingleton(new PricingConfiguration(
    100m,
    2m));
builder.Services.AddSingleton<PricingService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CarlyDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();   
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();

public partial class Program;
