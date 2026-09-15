using Carly.Api.Controllers.v1.Contracts.Request;
using Carly.Api.Features.Rental.Register;
using Carly.Api.Features.Rental.Register.Models;
using Carly.Api.Features.Shared.Models;
using Carly.Api.Infrastructure;
using Xunit;

namespace Carly.Api.Tests.UnitTests;

public sealed class RegisterVehicleTests
{
    [Fact]
    public void Execute_RegistersPickupAndTransitionsRental()
    {
        var store = new InMemoryRentalStore();
        var rental = TestHelpers.CreateBookedRental("BOOK-001");
        store.Add(rental);
        var service = new RegisterRentalService(store);
        var pickupDateTime = new DateTime(2026, 9, 12, 10, 30, 0);

        var result = service.Execute(
            "BOOK-001",
            new RegisterRequest("ABC123", "19800101-1234", CarCategory.SmallCar, pickupDateTime, 12000));

        Assert.Equal(RegisterRentalError.None, result.Error);
        Assert.Same(rental, result.Rental);
        Assert.Equal(RentalStatus.PickedUp, rental.Status);
        Assert.Equal("19800101-1234", rental.CustomerId);
        Assert.Equal(new RegisterRecord(pickupDateTime, 12000), rental.Pickup);
    }

    [Fact]
    public void Execute_ReturnsBookingNotFoundForUnknownBooking()
    {
        var service = new RegisterRentalService(new InMemoryRentalStore());

        var result = service.Execute("UNKNOWN", ValidRequest());

        Assert.Equal(RegisterRentalError.BookingNotFound, result.Error);
    }

    [Fact]
    public void Execute_RejectsAlreadyPickedUpRental()
    {
        var store = new InMemoryRentalStore();
        var rental = TestHelpers.CreateBookedRental("BOOK-001");
        rental.Status = RentalStatus.PickedUp;
        rental.Pickup = new RegisterRecord(DateTime.UtcNow, 10000);
        store.Add(rental);

        var result = new RegisterRentalService(store).Execute("BOOK-001", ValidRequest());

        Assert.Equal(RegisterRentalError.AlreadyPickedUp, result.Error);
    }

    [Fact]
    public void Execute_RejectsMismatchedRegistrationNumber()
    {
        var store = new InMemoryRentalStore();
        store.Add(TestHelpers.CreateBookedRental("BOOK-001"));

        var result = new RegisterRentalService(store).Execute(
            "BOOK-001",
            ValidRequest() with { RegistrationNumber = "XYZ999" });

        Assert.Equal(RegisterRentalError.RegistrationNumberMismatch, result.Error);
    }

    [Fact]
    public void Execute_RejectsMismatchedCategory()
    {
        var store = new InMemoryRentalStore();
        store.Add(TestHelpers.CreateBookedRental("BOOK-001"));

        var result = new RegisterRentalService(store).Execute(
            "BOOK-001",
            ValidRequest() with { Category = CarCategory.Truck });

        Assert.Equal(RegisterRentalError.CategoryMismatch, result.Error);
    }

    [Fact]
    public void Execute_RejectsInvalidInput()
    {
        var service = new RegisterRentalService(new InMemoryRentalStore());

        var result = service.Execute(
            "BOOK-001",
            ValidRequest() with { OdometerKm = -1 });

        Assert.Equal(RegisterRentalError.InvalidInput, result.Error);
    }

    [Fact]
    public async Task Execute_AllowsOnlyOneConcurrentPickup()
    {
        var store = new InMemoryRentalStore();
        store.Add(TestHelpers.CreateBookedRental("BOOK-001"));
        var requests = Enumerable.Range(0, 4)
            .Select(_ => Task.Run(() => new RegisterRentalService(store).Execute(
                "BOOK-001",
                ValidRequest())))
            .ToArray();

        var results = await Task.WhenAll(requests);

        Assert.Single(results, result => result.Error == RegisterRentalError.None);
        Assert.Equal(3, results.Count(result => result.Error == RegisterRentalError.AlreadyPickedUp));
    }

    private static RegisterRequest ValidRequest()
    {
        return new RegisterRequest(
            "ABC123",
            "19800101-1234",
            CarCategory.SmallCar,
            new DateTime(2026, 9, 12, 10, 30, 0),
            12000);
    }
}
