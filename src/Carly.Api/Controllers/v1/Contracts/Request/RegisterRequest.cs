using System.ComponentModel.DataAnnotations;
using Carly.Api.Features.Shared.Models;

namespace Carly.Api.Controllers.v1.Contracts.Request;

public sealed record RegisterRequest : IValidatableObject
{
    public RegisterRequest(
        DateTime pickupDateTime,
        int odometerKm)
    {
        PickupDateTime = pickupDateTime;
        OdometerKm = odometerKm;
    }
    
    [Required]
    public DateTime PickupDateTime { get; init; }

    [Range(0, int.MaxValue)]
    public int OdometerKm { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PickupDateTime == default)
            yield return new ValidationResult(
                "PickupDateTime must be supplied.",
                [nameof(PickupDateTime)]);
    }
}
