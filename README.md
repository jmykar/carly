# Carly

Small .NET monorepo for the Carly controller-based API and its tests.

## Projects

- `src/Carly.Api` - ASP.NET Core controller-based API
- `tests/Carly.Api.Tests` - API tests

## Run

```bash
dotnet run --project src/Carly.Api
```

The health endpoint is available at `/health`.

Swagger UI is available at `/swagger` and the OpenAPI document at
`/swagger/v1/swagger.json`.

Car pickup can be registered with `POST /api/v1/rental/{bookingNumber}` using the
registration number, customer identifier, car category, pickup date/time, and odometer
reading. 

Car return can be registered with `POST /api/v1/rental/{bookingNumber}/return` using
the return date/time and odometer reading. The final price is calculated and stored.

For API testing, a simplified booking can be registered with `POST /api/v1/booking`
using only the car category. The API generates a booking number such as `BOOK-1` and
a registration number such as `S1`. This endpoint is testing-only.

The API uses EF Core with SQLite for rental persistence. By default it creates or
updates `carly.db` in the working directory using the checked-in migrations. Set the
`ConnectionStrings:Carly` configuration value to use another database. Development
startup does not seed bookings.

## Documentation

- [Rental lifecycle use cases](docs/use-cases/rental-lifecycle.md)
- [Infrastructure decisions](docs/infra.md)

## Test

```bash
dotnet test Carly.slnx
```
