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

## Test

```bash
dotnet test Carly.slnx
```
