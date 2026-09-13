using System.Collections.Concurrent;
using Carly.Api.Features.Rental;
using RentalModel = Carly.Api.Features.Shared.Models.Rental;

namespace Carly.Api.Infrastructure;

public sealed class InMemoryRentalStore : IRentalStore
{
    private readonly ConcurrentDictionary<string, RentalModel> _rentals = new();

    public RentalModel? GetByBookingNumber(string bookingNumber)
    {
        _rentals.TryGetValue(bookingNumber, out var rental);
        return rental;
    }

    public void Add(RentalModel rental)
    {
        if (!_rentals.TryAdd(rental.BookingNumber, rental))
            throw new InvalidOperationException(
                $"A rental with booking number '{rental.BookingNumber}' already exists.");
    }

    public void Update(RentalModel rental)
    {
        if (!_rentals.ContainsKey(rental.BookingNumber))
            throw new KeyNotFoundException(
                $"No rental with booking number '{rental.BookingNumber}' exists.");

        _rentals[rental.BookingNumber] = rental;
    }
}
