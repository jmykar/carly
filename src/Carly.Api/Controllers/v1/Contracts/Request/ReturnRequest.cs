using System.ComponentModel.DataAnnotations;

namespace Carly.Api.Controllers.v1.Contracts.Request;

public sealed record ReturnRequest : IValidatableObject
{
    public ReturnRequest(DateTime returnDateTime, int odometerKm)
    {
        ReturnDateTime = returnDateTime;
        OdometerKm = odometerKm;
    }

    [Required]
    public DateTime ReturnDateTime { get; init; }

    [Range(0, int.MaxValue)]
    public int OdometerKm { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ReturnDateTime == default)
            yield return new ValidationResult(
                "ReturnDateTime must be supplied.",
                [nameof(ReturnDateTime)]);
    }
}
