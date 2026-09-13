using RentalModel = Carly.Api.Features.Shared.Models.Rental;

namespace Carly.Api.Features.Rental;

public interface IRentalStore
{
    RentalModel? GetByBookingNumber(string bookingNumber);

    void Add(RentalModel rental);

    void Update(RentalModel rental);
}
