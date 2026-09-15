# Carly agent instructions

## Scope

Carly is a small C#/.NET monorepo. Keep changes focused on the current use case and preserve the deliberately simple structure.

The initial domain scope is Rental Lifecycle Management: registering a car pickup, registering a returned car, and calculating the final rental price.

## Domain use cases

The rental lifecycle is:

```text
Booked -> Picked up -> Returned
```

- Pickup records the booking number, car registration number, customer identifier, car category, pickup date/time, and pickup odometer reading. Keep the API identifier generic and avoid exposing sensitive-data semantics in names unless explicitly required.
- Return records the booking number, return date/time, and return odometer reading, then calculates and stores the final price.
- A rental references exactly one car and is uniquely identified by its booking number.
- Bookings and cars are read from storage; their creation is outside the initial use-case scope. Simple test storage is acceptable.
- A rental cannot be returned before pickup, and a completed rental cannot be picked up or returned again.
- Distance is `return odometer reading - pickup odometer reading`; reject a lower return reading.
- Duration uses calendar days between pickup and return. Keep the exact boundary behavior explicit in tests.
- Currency is neutral for now. Keep rounding in the business-logic layer; the required precision must be made explicit before relying on monetary assertions.

## Pricing rules

Implement category-specific pricing in business logic, not in controllers or UI:

```text
Small car: baseDayRental * numberOfDays
Combi:     baseDayRental * numberOfDays * 1.3 + baseKmPrice * numberOfKm
Truck:     baseDayRental * numberOfDays * 1.5 + baseKmPrice * numberOfKm * 1.5
```

The category and base rates must be changeable without changing pickup and return workflows. In the current implementation, base rates are supplied through the injected `PricingConfiguration`; do not add runtime configuration or persistence without an explicit requirement. Additional categories should be addable without duplicating lifecycle logic. Unknown categories and missing price configuration must produce clear errors.

Do not expand the initial scope to correction workflows, detailed social-security-number access control, booking creation, car creation, or UI-specific behavior unless explicitly requested.

## Iterative delivery plan

Implement the system as small vertical slices. Each iteration should leave the solution buildable and should include the smallest complete path through domain logic, storage, API, and tests.

For every use-case iteration:

1. Define the acceptance criteria, inputs, preconditions, state changes, errors, and postconditions.
2. Add or update the smallest domain models and enums required by the use case.
3. Extend `IRentalStore` only with operations required by that slice.
4. Implement the use-case service and keep business rules out of the controller action.
5. Add the controller action and attribute route.
6. Add unit tests for business rules and controller/API tests for the request/response contract whenever the HTTP surface changes.
7. Run the focused tests, then `dotnet test Carly.slnx`.
8. Update documentation only when the externally visible behavior or setup changes.

Do not implement future use cases, database persistence, UI, or speculative abstractions as part of an earlier iteration.

### Iteration 0 — In-memory foundation

- Define the rental, booking reference, vehicle/category, pickup, and return representations needed by the first slice.
- Add `IRentalStore` and an `InMemoryRentalStore` implementation.
- Register the store through dependency injection as a singleton for local development and tests.
- If seed data is useful for local development, keep it minimal and clearly development-only. Tests should use setup helpers rather than depend on production seed data.
- Verify solution registration, compilation, and the existing health endpoint.

### Iteration 1 — Register car pickup

- Retrieve the booking and its assigned vehicle from the store.
- Validate that the booking exists, has one vehicle, and has not already been picked up.
- Store the booking number, registration number, customer identifier, category, pickup date/time, and pickup odometer reading.
- Transition the rental to `Picked up`.
- Test successful pickup and invalid booking, mismatched vehicle, duplicate pickup, missing data, invalid date/time, and invalid odometer cases.

### Iteration 2 — Rental price calculation

- Define the pricing input and result models.
- Implement category-specific pricing in a pricing service independent of HTTP and storage.
- Test Small car, Combi, and Truck calculations, configurable base rates, unknown categories, and the agreed rounding precision.
- Keep the currency neutral unless a later requirement introduces a currency contract.

### Iteration 3 — Register returned car

- Retrieve the rental by booking number and its stored pickup record.
- Validate that the booking exists, has been picked up, has not already been returned, and has a valid return date/time.
- Validate that the return odometer is not lower than the pickup reading.
- Calculate calendar-day duration and kilometres driven.
- Call the pricing service, store the return details and final price, and transition the rental to `Returned`.
- Test successful return and invalid booking, return-before-pickup, duplicate return, invalid dates, decreasing odometer, missing rates, and pricing failures.

### Infrastructure changes

Document infrastructure decisions and their implementation status in
[`docs/infra.md`](docs/infra.md). Preserve the `IRentalStore` contract where it still
represents the use case, keep technical integration code under `Infrastructure/`, and
rerun the same domain and API tests against any database-backed implementation.

## Repository layout

- Put deployable production projects under `src/`.
- Put automated tests under `tests/`.
- Keep `Carly.Api` as an ASP.NET Core controller-based API.
- Keep versioned HTTP contracts under `src/Carly.Api/Controllers/<Resource>/v<Version>/Contracts/`.
- Keep application and business logic under `src/Carly.Api/Features/<Resource>/`, organized by use case or capability, such as `Register/` and `Pricing/`.
- Keep the test project aligned with the feature structure where practical, with unit tests under `tests/Carly.Api.Tests/UnitTests/` and controller/API contract tests clearly separated when added.
- Use `support/ai/` for prompts, evaluation notes, scripts, and documentation while AI support is exploratory. Do not create a separate AI project until reusable production code or a second consumer justifies it.

## API organization

- Keep startup and dependency-injection composition in `src/Carly.Api/Program.cs`.
- Put HTTP controllers in `src/Carly.Api/Controllers/`.
- Use `[ApiController]`, attribute routing, constructor dependency injection, and `ActionResult` responses.
- Use explicit API versioned routes, currently `api/v1/rental`.
- Keep controllers thin: validate and translate HTTP input/output, then delegate business behavior to feature services.
- Keep controller contracts separate from feature/business models where practical. Do not introduce new dependencies from feature services or storage into versioned controller-contract namespaces.
- Add `Infrastructure/` only when there is actual technical integration code, such as persistence, external HTTP clients, authentication, telemetry, or storage.
- Do not introduce repositories, application layers, shared abstractions, or extra class libraries speculatively.

## Implementation conventions

- Follow ASP.NET Core controller conventions: `[ApiController]`, attribute routing, model binding, `ActionResult`, and dependency injection through constructors.
- Keep HTTP concerns in controllers and business logic in feature services.
- Keep pricing independent of HTTP, storage, and controller contracts; pass pricing inputs through feature-owned models.
- Use nullable reference types and implicit usings consistently with the existing projects.
- Keep package versions centrally managed in `Directory.Packages.props`.
- Keep shared compiler settings in `Directory.Build.props`.
- Match the repository's existing formatting and `.editorconfig` rules.

## Testing and verification

- Add or update tests for changed behavior, especially endpoint status codes and response contracts.
- Keep tests close to the corresponding feature under `tests/Carly.Api.Tests/`.
- Before considering a change complete, run the narrowest relevant test first, then run:

  ```bash
  dotnet test Carly.slnx
  ```

- If restore, credentials, network access, or another environment issue prevents verification, report the exact command and failure instead of claiming the change is verified.

## Change discipline

- Inspect the existing files and current working-tree state before editing.
- Make the smallest change that satisfies the request.
- Do not reorganize adjacent code or add speculative structure.
- Preserve unrelated user changes, including untracked files.
- State assumptions when a request could reasonably be interpreted in more than one way.

## References

Use Microsoft's ASP.NET Core controller/Web API documentation as the framework reference. The folder structure above is a project convention for maintainability, not a framework-mandated standard.
