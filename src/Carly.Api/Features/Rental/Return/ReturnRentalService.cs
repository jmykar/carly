using Carly.Api.Controllers.v1.Contracts.Request;
using Carly.Api.Features.Rental.Pricing;
using Carly.Api.Features.Rental.Pricing.Models;
using Carly.Api.Features.Rental.Return.Models;
using Carly.Api.Features.Shared.Models;
using ReturnRentalError = Carly.Api.Features.Rental.Return.Models.ReturnRentalError;

namespace Carly.Api.Features.Rental.Return;

public sealed class ReturnRentalService(
    IRentalStore rentalStore,
    PricingService pricingService)
{
    public ReturnRentalResult Execute(
        string bookingNumber,
        ReturnRequest request)
    {
        if (string.IsNullOrWhiteSpace(bookingNumber) ||
            request.ReturnDateTime == default ||
            request.OdometerKm < 0)
            return new ReturnRentalResult(null, ReturnRentalError.InvalidInput);

        var rental = rentalStore.GetByBookingNumber(bookingNumber);

        if (rental is null) return new ReturnRentalResult(null, ReturnRentalError.BookingNotFound);

        lock (rental)
        {
            if (rental.Status == RentalStatus.Returned || rental.Return is not null)
                return new ReturnRentalResult(null, ReturnRentalError.AlreadyReturned);

            if (rental.Status != RentalStatus.PickedUp || rental.Pickup is null)
                return new ReturnRentalResult(null, ReturnRentalError.NotPickedUp);

            if (request.ReturnDateTime < rental.Pickup.DateTime)
                return new ReturnRentalResult(null, ReturnRentalError.ReturnDateBeforePickup);

            if (request.OdometerKm < rental.Pickup.OdometerKm)
                return new ReturnRentalResult(null, ReturnRentalError.OdometerReadingTooLow);

            var numberOfDays = DateOnly.FromDateTime(request.ReturnDateTime).DayNumber
                               - DateOnly.FromDateTime(rental.Pickup.DateTime).DayNumber;
            var numberOfKm = request.OdometerKm - rental.Pickup.OdometerKm;

            PricingResult pricingResult;
            try
            {
                pricingResult = pricingService.Calculate(new PricingFactor(
                    rental.Category,
                    numberOfDays,
                    numberOfKm));
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ReturnRentalResult(null, ReturnRentalError.PricingFailed);
            }

            rental.Return = new ReturnRecord(request.ReturnDateTime, request.OdometerKm);
            rental.Price = pricingResult.Price;
            rental.Status = RentalStatus.Returned;
            rentalStore.Update(rental);
        }

        return new ReturnRentalResult(rental);
    }
}
