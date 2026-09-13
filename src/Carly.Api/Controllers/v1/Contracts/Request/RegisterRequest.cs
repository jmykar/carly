using System.ComponentModel.DataAnnotations;
using Carly.Api.Features.Shared.Models;

namespace Carly.Api.Controllers.v1.Contracts.Request;

public sealed record RegisterRequest : IValidatableObject
{
    public RegisterRequest(
        string registrationNumber,
        string customerId,
        CarCategory category,
        DateTime pickupDateTime,
        int odometerKm)
    {
        RegistrationNumber = registrationNumber;
        CustomerId = customerId;
        Category = category;
        PickupDateTime = pickupDateTime;
        OdometerKm = odometerKm;
    }

    [Required(AllowEmptyStrings = false)]
    public string RegistrationNumber { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string CustomerId { get; init; }

    [EnumDataType(typeof(CarCategory))]
    public CarCategory Category { get; init; }

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
