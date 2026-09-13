using Carly.Api.Features.Rental.Pricing;
using Carly.Api.Features.Rental.Pricing.Models;
using Carly.Api.Features.Shared.Models;
using Xunit;

namespace Carly.Api.Tests.UnitTests;

public sealed class PricingServiceTests
{
    private readonly PricingService _service = new(new PricingConfiguration(
        BaseDayPrice: 100m,
        BaseKmPrice: 2m));

    [Fact]
    public void Calculate_SmallCar_UsesDailyRateOnly()
    {
        var result = _service.Calculate(new(
            CarCategory.SmallCar,
            NumberOfDays: 3,
            NumberOfKm: 250));

        Assert.Equal(300m, result.Price);
    }

    [Fact]
    public void Calculate_Combi_UsesDailyAndKilometreRates()
    {
        var result = _service.Calculate(new(
            CarCategory.Combi,
            NumberOfDays: 3,
            NumberOfKm: 250));

        Assert.Equal(890m, result.Price);
    }

    [Fact]
    public void Calculate_Truck_AppliesBothTruckFactors()
    {
        var result = _service.Calculate(new(
            CarCategory.Truck,
            NumberOfDays: 3,
            NumberOfKm: 250));

        Assert.Equal(1_200m, result.Price);
    }

    [Fact]
    public void Calculate_RoundsToTwoDecimalPlaces()
    {
        var service = new PricingService(new PricingConfiguration(
            BaseDayPrice: 10.005m,
            BaseKmPrice: 0m));
        var result = service.Calculate(new(
            CarCategory.Combi,
            NumberOfDays: 1,
            NumberOfKm: 1));

        Assert.Equal(13.01m, result.Price);
    }

    [Fact]
    public void Calculate_RejectsNegativeInputs()
    {
        var request = new PricingFactor(
            CarCategory.SmallCar,
            NumberOfDays: -1,
            NumberOfKm: 0);

        Assert.Throws<ArgumentOutOfRangeException>(() => _service.Calculate(request));
    }

    [Fact]
    public void Calculate_RejectsUnknownCategory()
    {
        var request = new PricingFactor(
            (CarCategory)999,
            NumberOfDays: 1,
            NumberOfKm: 0);

        Assert.Throws<ArgumentOutOfRangeException>(() => _service.Calculate(request));
    }

    [Fact]
    public void Constructor_RejectsMissingConfiguration()
    {
        Assert.Throws<ArgumentNullException>(() => new PricingService(null));
    }

    [Fact]
    public void Constructor_RejectsNegativeConfigurationRates()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PricingService(new PricingConfiguration(-1m, 2m)));
    }
}
