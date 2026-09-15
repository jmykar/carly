# Rental Lifecycle Management

This document is the living domain reference for Carly's rental use cases. It describes the intended behavior independently of the current storage implementation and UI.

## Status

| Use case | Description | Status |
| --- | --- | --- |
| UC-01 | Register car pickup | Implemented |
| UC-02 | Calculate rental price | Implemented |
| UC-03 | Register returned car | Implemented |

## Goal and actors

Carly supports a rental agent registering vehicle pickup and return, calculating the final rental price, and storing the rental lifecycle data.

The primary actor is a rental agent. Bookings and assigned vehicles are read from storage; creating them is outside the current scope.

## Lifecycle

```text
Booked -> PickedUp -> Returned
```

A rental has one booking number and references exactly one vehicle. A rental cannot be returned before pickup, and a completed rental cannot be picked up or returned again.

## UC-01 — Register car pickup

### Preconditions

- The booking exists in storage.
- The booking has one assigned vehicle.
- The rental is in `Booked` state.

### Input

- Booking number (route value)
- Vehicle registration number
- Customer identifier
- Car category
- Pickup date and time
- Pickup odometer reading in kilometres

### Main flow

1. Retrieve the rental by booking number.
2. Validate that the vehicle registration number and category match the stored rental.
3. Validate the required values, pickup date/time, and non-negative odometer reading.
4. Store the customer identifier and pickup record.
5. Transition the rental to `PickedUp`.

### Rejections

Return a clear failure when the booking is unknown, already picked up, the vehicle or category does not match, required input is missing, the pickup date/time is invalid, or the odometer reading is negative.

### Current API

```text
POST /api/v1/rental/{bookingNumber}
```

The route intentionally does not include `/pickup`; the operation is described as
vehicle pickup registration in the generated OpenAPI document.

The controller translates use-case outcomes into HTTP responses; validation and state changes belong to the feature service.

## UC-02 — Calculate rental price

This is a business-logic use case used when a rental is returned. It has no separate HTTP endpoint.

### Inputs

- Car category
- Number of calendar days
- Number of kilometres driven
- Configured `baseDayPrice`
- Configured `baseKmPrice`

### Pricing rules

```text
Small car = baseDayPrice * numberOfDays
Combi     = baseDayPrice * numberOfDays * 1.3
            + baseKmPrice * numberOfKm
Truck     = baseDayPrice * numberOfDays * 1.5
            + baseKmPrice * numberOfKm * 1.5
```

Prices are represented as decimal values and rounded to two decimal places using midpoint-away-from-zero rounding. Currency remains neutral until a currency contract is introduced.

Negative duration, distance, or configured rates are invalid. Unknown categories and missing pricing configuration must fail clearly.

## UC-03 — Register returned car

### Preconditions

- The booking exists in storage.
- The rental is in `PickedUp` state.
- A pickup record exists.
- The rental has not already been returned.

### Input

- Booking number (route value)
- Return date and time
- Return odometer reading in kilometres

### Main flow

1. Retrieve the rental and stored pickup record by booking number.
2. Validate the lifecycle state and return input.
3. Calculate calendar days as the difference between the pickup and return calendar dates.
4. Calculate kilometres as:

   ```text
   return odometer reading - pickup odometer reading
   ```

5. Calculate the price using UC-02.
6. Store the return record and calculated price.
7. Transition the rental to `Returned`.

### Rejections

Return a clear failure when the booking is unknown, has not been picked up, has already been returned, the return date/time is before pickup, the return odometer reading is lower than pickup, the input is invalid, or pricing fails.

### Current API

```text
POST /api/v1/rental/{bookingNumber}/return
```

## Scope boundaries

The following are intentionally outside the current use cases:

- Creating bookings or vehicles
- Correcting pickup or return registrations
- Detailed access-control rules for customer identifiers
- UI-specific behavior
- Durable persistence is implemented behind the existing storage contract; future
  persistence work is limited to server-hosted or cloud databases.
