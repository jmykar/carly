# Infrastructure

This document records technical infrastructure decisions and their implementation
status. It does not define user-facing domain behavior.

## INF-01 — Durable local persistence with EF Core and SQLite

Status: Implemented

Carly uses EF Core with SQLite for durable local rental storage.

### Implementation

- `EfRentalStore` implements `IRentalStore`.
- `CarlyDbContext` and persistence entities live under `Infrastructure/`.
- SQLite is the default local database.
- The default database file is `carly.db`.
- `ConnectionStrings:Carly` can override the database location.
- EF Core migrations are checked into source control.
- The `InitialCreate` migration is checked into source control.
- Development startup applies migrations.
- Booking registration for API testing uses a SQLite-generated integer identity as the
  sequence source, formatting it as `BOOK-{Id}` and `SE{Id}`.
- Persistence tests use isolated in-memory SQLite connections.
- The generated integer `Id` is the primary key and `BookingNumber` has a unique index.

### Acceptance criteria

- Rental state persists across application restarts.
- Existing pickup, pricing, and return behavior is unchanged.
- `IRentalStore` consumers do not depend on EF Core types.
- The schema is reproducible from committed migrations.
- Development seed data is not required by tests.
- Unit, persistence, and API contract tests pass.

### Non-goals

- Server-hosted or cloud databases
- Production booking or vehicle creation
- API route or response-contract changes
- Generic repository or unit-of-work abstractions
