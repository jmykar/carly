namespace Carly.Api.Features.Shared.Models;

public sealed class Rental
{
    public required string BookingNumber { get; init; }

    public required string RegistrationNumber { get; init; }

    public required CarCategory Category { get; init; }

    public RentalStatus Status { get; set; } = RentalStatus.Booked;

    public string? CustomerId { get; set; }

    public RegisterRecord? Pickup { get; set; }

    public ReturnRecord? Return { get; set; }

    public decimal? Price { get; set; }
}

public sealed record RegisterRecord(DateTime DateTime, int OdometerKm);

public sealed record ReturnRecord(DateTime DateTime, int OdometerKm);
