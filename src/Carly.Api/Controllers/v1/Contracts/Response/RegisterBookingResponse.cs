using Carly.Api.Features.Shared.Models;

namespace Carly.Api.Controllers.v1.Contracts.Response;

public sealed record RegisterBookingResponse(
    string BookingNumber,
    string RegistrationNumber,
    CarCategory Category);
