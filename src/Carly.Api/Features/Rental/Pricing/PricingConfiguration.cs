namespace Carly.Api.Features.Rental.Pricing;

public sealed record PricingConfiguration(
    decimal BaseDayPrice,
    decimal BaseKmPrice);
