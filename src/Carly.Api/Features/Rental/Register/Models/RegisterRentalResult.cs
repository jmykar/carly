using RentalModel = Carly.Api.Features.Shared.Models.Rental;

namespace Carly.Api.Features.Rental.Register.Models;

public sealed record RegisterRentalResult(
    RentalModel? Rental,
    RegisterRentalError Error = RegisterRentalError.None);
