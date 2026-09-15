using Carly.Api.Features.Shared.Models;
using Carly.Api.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Carly.Api.Tests.UnitTests.Infrastructure;

public sealed class EfRentalStoreTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly CarlyDbContext _dbContext;

    public EfRentalStoreTests()
    {
        _connection.Open();
        _dbContext = new CarlyDbContext(new DbContextOptionsBuilder<CarlyDbContext>()
            .UseSqlite(_connection)
            .Options);
        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public void AddAndGet_RoundTripsRentalLifecycleData()
    {
        var store = new EfRentalStore(_dbContext);
        var rental = TestHelpers.CreateBookedRental("BOOK-001");
        rental.CustomerId = "customer";
        rental.Pickup = new RegisterRecord(new DateTime(2026, 9, 14, 10, 0, 0), 100);
        rental.Status = RentalStatus.PickedUp;

        store.Add(rental);

        var result = store.GetByBookingNumber("BOOK-001");

        Assert.NotNull(result);
        Assert.Equal("customer", result.CustomerId);
        Assert.Equal(rental.Pickup, result.Pickup);
        Assert.Equal(RentalStatus.PickedUp, result.Status);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

}
