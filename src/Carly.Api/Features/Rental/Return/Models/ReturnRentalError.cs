namespace Carly.Api.Features.Rental.Return.Models;

public enum ReturnRentalError
{
    None,
    BookingNotFound,
    NotPickedUp,
    AlreadyReturned,
    ReturnDateBeforePickup,
    OdometerReadingTooLow,
    InvalidInput,
    PricingFailed
}
