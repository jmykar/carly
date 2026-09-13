using Carly.Api.Features.Shared.Models;
using Carly.Api.Infrastructure;
using Xunit;

namespace Carly.Api.Tests.UnitTests.Infrastructure;

public sealed class InMemoryRentalStoreTests
{
    [Fact]
    public void GetByBookingNumber_ReturnsStoredRental()
    {
        var store = new InMemoryRentalStore();
        var rental = TestHelpers.CreateBookedRental("BOOK-001");

        store.Add(rental);

        Assert.Same(rental, store.GetByBookingNumber("BOOK-001"));
    }

    [Fact]
    public void GetByBookingNumber_ReturnsNullForUnknownBooking()
    {
        var store = new InMemoryRentalStore();

        var result = store.GetByBookingNumber("UNKNOWN");

        Assert.Null(result);
    }

    [Fact]
    public void Add_RejectsDuplicateBookingNumber()
    {
        var store = new InMemoryRentalStore();
        store.Add(TestHelpers.CreateBookedRental("BOOK-001"));

        var action = () => store.Add(TestHelpers.CreateBookedRental("BOOK-001"));

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Update_ReplacesExistingRental()
    {
        var store = new InMemoryRentalStore();
        store.Add(TestHelpers.CreateBookedRental("BOOK-001"));
        var updatedRental = TestHelpers.CreateBookedRental("BOOK-001");
        updatedRental.Status = RentalStatus.PickedUp;

        store.Update(updatedRental);

        Assert.Same(updatedRental, store.GetByBookingNumber("BOOK-001"));
        Assert.Equal(RentalStatus.PickedUp, store.GetByBookingNumber("BOOK-001")!.Status);
    }

    [Fact]
    public void Update_RejectsUnknownBookingNumber()
    {
        var store = new InMemoryRentalStore();
        var action = () => store.Update(TestHelpers.CreateBookedRental("UNKNOWN"));

        Assert.Throws<KeyNotFoundException>(action);
    }
}
