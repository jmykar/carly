using Carly.Api.Features.Rental;
using Carly.Api.Features.Shared.Models;
using Carly.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using RentalModel = Carly.Api.Features.Shared.Models.Rental;

namespace Carly.Api.Infrastructure;

public sealed class EfRentalStore(CarlyDbContext dbContext) : IRentalStore
{
    public RentalModel? GetByBookingNumber(string bookingNumber)
    {
        var entity = dbContext.Rentals.SingleOrDefault(rental => rental.BookingNumber == bookingNumber);
        return entity is null ? null : ToModel(entity);
    }

    public RentalModel Add(RentalModel rental)
    {
        using var transaction = dbContext.Database.BeginTransaction();
        var entity = ToEntity(rental);
        dbContext.Rentals.Add(entity);

        try
        {
            dbContext.SaveChanges();
            transaction.Commit();
            return ToModel(entity);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException(
                $"A rental with booking number '{rental.BookingNumber}' already exists.", exception);
        }
    }

    public void Update(RentalModel rental)
    {
        var entity = dbContext.Rentals.SingleOrDefault(item => item.BookingNumber == rental.BookingNumber)
                     ?? throw new KeyNotFoundException(
                         $"No rental with booking number '{rental.BookingNumber}' exists.");

        entity.RegistrationNumber = rental.RegistrationNumber;
        entity.Category = rental.Category;
        entity.Status = rental.Status;
        entity.CustomerId = rental.CustomerId;
        entity.PickupDateTime = rental.Pickup?.DateTime;
        entity.PickupOdometerKm = rental.Pickup?.OdometerKm;
        entity.ReturnDateTime = rental.Return?.DateTime;
        entity.ReturnOdometerKm = rental.Return?.OdometerKm;
        entity.Price = rental.Price;
        dbContext.SaveChanges();
    }

    private static RentalEntity ToEntity(RentalModel rental) => new()
    {
        BookingNumber = rental.BookingNumber,
        RegistrationNumber = rental.RegistrationNumber,
        Category = rental.Category,
        Status = rental.Status,
        CustomerId = rental.CustomerId,
        PickupDateTime = rental.Pickup?.DateTime,
        PickupOdometerKm = rental.Pickup?.OdometerKm,
        ReturnDateTime = rental.Return?.DateTime,
        ReturnOdometerKm = rental.Return?.OdometerKm,
        Price = rental.Price
    };

    private static RentalModel ToModel(RentalEntity entity) => new()
    {
        BookingNumber = entity.BookingNumber,
        RegistrationNumber = entity.RegistrationNumber,
        Category = entity.Category,
        Status = entity.Status,
        CustomerId = entity.CustomerId,
        Pickup = entity.PickupDateTime is { } pickupDateTime && entity.PickupOdometerKm is { } pickupOdometer
            ? new RegisterRecord(pickupDateTime, pickupOdometer)
            : null,
        Return = entity.ReturnDateTime is { } returnDateTime && entity.ReturnOdometerKm is { } returnOdometer
            ? new ReturnRecord(returnDateTime, returnOdometer)
            : null,
        Price = entity.Price
    };
}
