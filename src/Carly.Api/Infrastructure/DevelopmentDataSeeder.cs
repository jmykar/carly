using Carly.Api.Features.Rental;
using Carly.Api.Features.Shared.Models;

namespace Carly.Api.Infrastructure;

public static class DevelopmentDataSeeder
{
    public static void Seed(IRentalStore rentalStore)
    {
        AddIfMissing(rentalStore, "BOOK-001", "CAR-1", CarCategory.SmallCar);
        AddIfMissing(rentalStore, "BOOK-002", "COMBI-1", CarCategory.Combi);
        AddIfMissing(rentalStore, "BOOK-003", "TRUCK-1", CarCategory.Truck);
    }

    private static void AddIfMissing(
        IRentalStore rentalStore,
        string bookingNumber,
        string registrationNumber,
        CarCategory category)
    {
        if (rentalStore.GetByBookingNumber(bookingNumber) is not null)
            return;

        rentalStore.Add(new Rental
        {
            BookingNumber = bookingNumber,
            RegistrationNumber = registrationNumber,
            Category = category
        });
    }
}
