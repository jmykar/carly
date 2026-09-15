using Carly.Api.Features.Rental.Pricing.Models;
using Carly.Api.Features.Shared.Models;

namespace Carly.Api.Features.Rental.Pricing;

public sealed class PricingService
{
    private const int RoundingDigits = 2;
    private readonly PricingConfiguration _configuration;

    public PricingService(PricingConfiguration? configuration)
    {
        _configuration = configuration
                         ?? throw new ArgumentNullException(nameof(configuration));

        if (configuration.BaseDayPrice < 0)
            throw new ArgumentOutOfRangeException(
                nameof(configuration.BaseDayPrice));

        if (configuration.BaseKmPrice < 0)
            throw new ArgumentOutOfRangeException(
                nameof(configuration.BaseKmPrice));
    }

    public PricingResult Calculate(PricingFactor? request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.NumberOfDays < 0) throw new ArgumentOutOfRangeException(nameof(request.NumberOfDays));

        if (request.NumberOfKm < 0) throw new ArgumentOutOfRangeException(nameof(request.NumberOfKm));

        var price = request.Category switch
        {
            CarCategory.SmallCar => CalculateDailyPrice(request),
            CarCategory.Combi => CalculateDailyPrice(request) * 1.3m
                                 + CalculateDistancePrice(request),
            CarCategory.Truck => CalculateDailyPrice(request) * 1.5m
                                 + CalculateDistancePrice(request) * 1.5m,
            _ => throw new ArgumentOutOfRangeException(
                nameof(request.Category),
                request.Category,
                "The car category is not supported.")
        };

        return new PricingResult(Math.Round(price, RoundingDigits, MidpointRounding.AwayFromZero));
    }

    private decimal CalculateDistancePrice(PricingFactor request)
    {
        return _configuration.BaseKmPrice * request.NumberOfKm;
    }

    private decimal CalculateDailyPrice(PricingFactor request)
    {
        return _configuration.BaseDayPrice * request.NumberOfDays;
    }
}
