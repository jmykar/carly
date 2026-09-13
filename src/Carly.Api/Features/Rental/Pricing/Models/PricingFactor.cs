using Carly.Api.Features.Shared.Models;

namespace Carly.Api.Features.Rental.Pricing.Models;

public sealed record PricingFactor(
    CarCategory Category,
    int NumberOfDays,
    int NumberOfKm);
