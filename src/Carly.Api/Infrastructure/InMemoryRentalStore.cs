using System.Collections.Concurrent;
using Carly.Api.Features.Rental;
using Carly.Api.Features.Shared.Models;
using RentalModel = Carly.Api.Features.Shared.Models.Rental;

namespace Carly.Api.Infrastructure;

public sealed class InMemoryRentalStore : IRentalStore
{
    private readonly ConcurrentDictionary<string, RentalModel> _rentals = new();
    private int _nextIdentifier;

    public RentalModel? GetByBookingNumber(string bookingNumber)
    {
        _rentals.TryGetValue(bookingNumber, out var rental);
        return rental;
    }

    public RentalModel Add(RentalModel rental)
    {
        if (string.IsNullOrWhiteSpace(rental.BookingNumber))
        {
            var identifier = Interlocked.Increment(ref _nextIdentifier);
            rental = new RentalModel
            {
                BookingNumber = $"BOOK-{identifier}",
                RegistrationNumber = $"S{identifier}",
                Category = rental.Category,
                Status = rental.Status,
                CustomerId = rental.CustomerId,
                Pickup = rental.Pickup,
                Return = rental.Return,
                Price = rental.Price
            };
        }

        if (!_rentals.TryAdd(rental.BookingNumber, rental))
            throw new InvalidOperationException(
                $"A rental with booking number '{rental.BookingNumber}' already exists.");

        return rental;
    }

    public void Update(RentalModel rental)
    {
        if (!_rentals.ContainsKey(rental.BookingNumber))
            throw new KeyNotFoundException(
                $"No rental with booking number '{rental.BookingNumber}' exists.");

        _rentals[rental.BookingNumber] = rental;
    }
}
