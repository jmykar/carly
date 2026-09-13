using Carly.Api.Controllers.v1.Contracts.Request;
using Carly.Api.Features.Rental.Register.Models;
using Carly.Api.Features.Shared.Models;

namespace Carly.Api.Features.Rental.Register;

public sealed class RegisterVehicleService(IRentalStore rentalStore)
{
    public RegisterRentalResult Execute(
        string bookingNumber,
        RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(bookingNumber) ||
            string.IsNullOrWhiteSpace(request.RegistrationNumber) ||
            string.IsNullOrWhiteSpace(request.CustomerId) ||
            request.PickupDateTime == default ||
            request.OdometerKm < 0)
            return new RegisterRentalResult(null, RegisterRentalError.InvalidInput);

        var rental = rentalStore.GetByBookingNumber(bookingNumber);

        if (rental is null) return new RegisterRentalResult(null, RegisterRentalError.BookingNotFound);

        lock (rental)
        {
            if (rental.Status != RentalStatus.Booked || rental.Pickup is not null)
                return new RegisterRentalResult(null, RegisterRentalError.AlreadyPickedUp);

            if (!string.Equals(
                    rental.RegistrationNumber,
                    request.RegistrationNumber,
                    StringComparison.OrdinalIgnoreCase))
                return new RegisterRentalResult(null, RegisterRentalError.RegistrationNumberMismatch);

            if (rental.Category != request.Category)
                return new RegisterRentalResult(null, RegisterRentalError.CategoryMismatch);

            rental.CustomerId = request.CustomerId;
            rental.Pickup = new RegisterRecord(request.PickupDateTime, request.OdometerKm);
            rental.Status = RentalStatus.PickedUp;
            rentalStore.Update(rental);
        }

        return new RegisterRentalResult(rental);
    }
}
