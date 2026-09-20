using Carly.Api.Controllers.v1.Contracts.Request;
using Carly.Api.Controllers.v1.Contracts.Response;
using Carly.Api.Features.Rental;
using Microsoft.AspNetCore.Mvc;

namespace Carly.Api.Controllers.v1;

[ApiController]
[Route("api/v1/booking")]
public sealed class BookingController(IRentalStore rentalStore) : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RegisterBookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Register booking for testing")]
    [EndpointDescription("Registers a simplified booking for API testing and generates its booking and registration numbers.")]
    public ActionResult<RegisterBookingResponse> Register(RegisterBookingRequest request)
    {
        var rental = rentalStore.Add(new Features.Shared.Models.Rental
        {
            BookingNumber = $"BOOK-{Guid.NewGuid()}",
            RegistrationNumber = $"SE{Random.Shared.Next(1000, 9999)}",
            CustomerId = request.CustomerId,
            Category = request.Category!.Value
        });
        
        var response = new RegisterBookingResponse(
            rental.BookingNumber,
            rental.RegistrationNumber,
            rental.Category);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}
