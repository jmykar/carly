using Carly.Api.Controllers.v1.Contracts.Request;
using Carly.Api.Features.Rental.Pricing;
using Carly.Api.Features.Rental.Return;
using Carly.Api.Features.Rental.Return.Models;
using Carly.Api.Features.Shared.Models;
using Carly.Api.Infrastructure;
using Xunit;

namespace Carly.Api.Tests.UnitTests;

public sealed class RegisterReturnServiceTests
{
    private readonly PricingService _pricingService = new(new PricingConfiguration(100m, 2m));

    [Fact]
    public void Execute_RegistersReturnCalculatesPriceAndTransitionsRental()
    {
        var store = new InMemoryRentalStore();
        var rental = TestHelpers.CreateBookedRental("BOOK-001");
        rental.Status = RentalStatus.PickedUp;
        rental.Pickup = new RegisterRecord(new DateTime(2026, 9, 12, 10, 30, 0), 12000);
        store.Add(rental);

        var result = CreateService(store).Execute(
            "BOOK-001",
            new ReturnRequest(new DateTime(2026, 9, 14, 9, 0, 0), 12500));

        Assert.Equal(ReturnRentalError.None, result.Error);
        Assert.Equal(RentalStatus.Returned, rental.Status);
        Assert.Equal(new ReturnRecord(new DateTime(2026, 9, 14, 9, 0, 0), 12500), rental.Return);
        Assert.Equal(200m, rental.Price);
    }

    [Fact]
    public void Execute_UsesCalendarDaysForDuration()
    {
        var store = CreatePickedUpStore();

        var result = CreateService(store).Execute(
            "BOOK-001",
            new ReturnRequest(new DateTime(2026, 9, 13, 9, 0, 0), 12000));

        Assert.Equal(ReturnRentalError.None, result.Error);
        Assert.Equal(100m, result.Rental!.Price);
    }

    [Fact]
    public void Execute_ReturnsBookingNotFoundForUnknownBooking()
    {
        var result = CreateService(new InMemoryRentalStore()).Execute(
            "UNKNOWN",
            new ReturnRequest(DateTime.UtcNow, 12000));

        Assert.Equal(ReturnRentalError.BookingNotFound, result.Error);
    }

    [Fact]
    public void Execute_RejectsReturnBeforePickup()
    {
        var result = CreateService(CreatePickedUpStore()).Execute(
            "BOOK-001",
            new ReturnRequest(new DateTime(2026, 9, 12, 10, 29, 59), 12000));

        Assert.Equal(ReturnRentalError.ReturnDateBeforePickup, result.Error);
    }

    [Fact]
    public void Execute_RejectsLowerReturnOdometerReading()
    {
        var result = CreateService(CreatePickedUpStore()).Execute(
            "BOOK-001",
            new ReturnRequest(new DateTime(2026, 9, 12, 11, 0, 0), 11999));

        Assert.Equal(ReturnRentalError.OdometerReadingTooLow, result.Error);
    }

    [Fact]
    public void Execute_RejectsRentalThatHasNotBeenPickedUp()
    {
        var store = new InMemoryRentalStore();
        store.Add(TestHelpers.CreateBookedRental("BOOK-001"));

        var result = CreateService(store).Execute(
            "BOOK-001",
            new ReturnRequest(DateTime.UtcNow, 12000));

        Assert.Equal(ReturnRentalError.NotPickedUp, result.Error);
    }

    [Fact]
    public void Execute_RejectsAlreadyReturnedRental()
    {
        var store = CreatePickedUpStore();
        var rental = store.GetByBookingNumber("BOOK-001")!;
        rental.Status = RentalStatus.Returned;
        rental.Return = new ReturnRecord(DateTime.UtcNow, 12000);

        var result = CreateService(store).Execute(
            "BOOK-001",
            new ReturnRequest(DateTime.UtcNow, 12000));

        Assert.Equal(ReturnRentalError.AlreadyReturned, result.Error);
    }

    private ReturnRentalService CreateService(InMemoryRentalStore store)
    {
        return new ReturnRentalService(store, _pricingService);
    }

    private static InMemoryRentalStore CreatePickedUpStore()
    {
        var store = new InMemoryRentalStore();
        var rental = TestHelpers.CreateBookedRental("BOOK-001");
        rental.Status = RentalStatus.PickedUp;
        rental.Pickup = new RegisterRecord(new DateTime(2026, 9, 12, 10, 30, 0), 12000);
        store.Add(rental);
        return store;
    }
}
