using Carly.Api.Features.Shared.Models;
using RentalModel = Carly.Api.Features.Shared.Models.Rental;

namespace Carly.Api.Features.Rental;

public interface IRentalStore
{
    RentalModel? GetByBookingNumber(string bookingNumber);

    RentalModel Add(RentalModel rental);

    void Update(RentalModel rental);
}
