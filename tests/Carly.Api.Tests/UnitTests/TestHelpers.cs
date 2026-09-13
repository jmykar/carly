using Carly.Api.Features.Shared.Models;
using RentalModel = Carly.Api.Features.Shared.Models.Rental;

namespace Carly.Api.Tests.UnitTests;

internal static class TestHelpers
{
    public static RentalModel CreateBookedRental(string bookingNumber)
    {
        return new RentalModel
        {
            BookingNumber = bookingNumber,
            RegistrationNumber = "ABC123",
            Category = CarCategory.SmallCar
        };
    }
}
