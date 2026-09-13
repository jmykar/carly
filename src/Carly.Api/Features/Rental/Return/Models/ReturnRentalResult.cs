using RentalModel = Carly.Api.Features.Shared.Models.Rental;

namespace Carly.Api.Features.Rental.Return.Models;

public sealed record ReturnRentalResult(
    RentalModel? Rental,
    ReturnRentalError Error = ReturnRentalError.None);

