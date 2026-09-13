using Carly.Api.Controllers.v1.Contracts.Request;
using Carly.Api.Features.Rental.Register;
using Carly.Api.Features.Rental.Register.Models;
using Carly.Api.Features.Rental.Return;
using Carly.Api.Features.Rental.Return.Models;
using Microsoft.AspNetCore.Mvc;
using RentalModel = Carly.Api.Features.Shared.Models.Rental;

namespace Carly.Api.Controllers.v1;

[ApiController]
[Route("api/v1/rental")]
public sealed class RentalController(
    RegisterVehicleService registerService,
    ReturnVehicleService returnService) : ControllerBase
{
    [HttpPost("{bookingNumber}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RentalModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Register vehicle pickup")]
    [EndpointDescription("Registers the pickup of the vehicle assigned to the booking.")]
    public ActionResult<RentalModel> Register(
        string bookingNumber,
        RegisterRequest request)
    {
        var result = registerService.Execute(bookingNumber, request);

        return result.Error switch
        {
            RegisterRentalError.None => Ok(result.Rental),
            RegisterRentalError.BookingNotFound => Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Booking not found",
                detail: "No rental exists for the supplied booking number."),
            RegisterRentalError.AlreadyPickedUp => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Rental conflict",
                detail: "The rental has already been picked up."),
            RegisterRentalError.RegistrationNumberMismatch => Problem(
                detail: "The registration number does not match the booking."),
            RegisterRentalError.CategoryMismatch => Problem(
                detail: "The car category does not match the booking."),
            RegisterRentalError.InvalidInput => Problem(
                detail: "The pickup information is invalid."),
            _ => Problem(detail: "The pickup could not be registered.")
        };
    }

    [HttpPost("{bookingNumber}/return")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RentalModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Register vehicle return")]
    [EndpointDescription("Registers the return of a picked-up vehicle and stores the final rental price.")]
    public ActionResult<RentalModel> Return(
        string bookingNumber,
        ReturnRequest request)
    {
        var result = returnService.Execute(bookingNumber, request);

        return result.Error switch
        {
            ReturnRentalError.None => Ok(result.Rental),
            ReturnRentalError.BookingNotFound => Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Booking not found",
                detail: "No rental exists for the supplied booking number."),
            ReturnRentalError.NotPickedUp => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Rental conflict",
                detail: "The rental has not been picked up."),
            ReturnRentalError.AlreadyReturned => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Rental conflict",
                detail: "The rental has already been returned."),
            ReturnRentalError.ReturnDateBeforePickup => Problem(
                detail: "The return date/time cannot be before pickup."),
            ReturnRentalError.OdometerReadingTooLow => Problem(
                detail: "The return odometer reading cannot be lower than pickup."),
            ReturnRentalError.InvalidInput => Problem(
                detail: "The return information is invalid."),
            ReturnRentalError.PricingFailed => Problem(
                detail: "The rental price could not be calculated."),
            _ => Problem(detail: "The return could not be registered.")
        };
    }
}
