namespace Carly.Api.Features.Rental.Register.Models;

public enum RegisterRentalError
{
    None,
    BookingNotFound,
    AlreadyPickedUp,
    RegistrationNumberMismatch,
    CategoryMismatch,
    InvalidInput
}
