import { describe, expect, it, vi } from "vitest";
import { ApiProblem } from "./schema-helper";
import { type FetchFunction, RentalService } from "./rental-service";

function jsonResponse(body: unknown, status = 200): Response {
  return new Response(JSON.stringify(body), {
    status,
    headers: { "Content-Type": "application/json" }
  });
}

describe("RentalService", () => {
  it("posts the booking contract and returns the generated identifiers", async () => {
    const fetchFunction = vi.fn<FetchFunction>().mockResolvedValue(jsonResponse({
      bookingNumber: "BOOK-1",
      registrationNumber: "S1",
      category: "SmallCar"
    }, 201));
    const client = new RentalService("/api", fetchFunction);

    await expect(client.createBooking({ category: "SmallCar", customerId: "customer-1" }))
      .resolves.toEqual({
        bookingNumber: "BOOK-1",
        registrationNumber: "S1",
        category: "SmallCar"
      });

    expect(fetchFunction).toHaveBeenCalledWith("/api/v1/booking", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ category: "SmallCar", customerId: "customer-1" })
    });
  });

  it("encodes the pickup booking number and reads its JSON string response", async () => {
    const fetchFunction = vi.fn<FetchFunction>().mockResolvedValue(
      jsonResponse("Vehicle picked up successfully.")
    );
    const client = new RentalService("/api", fetchFunction);

    await expect(client.registerPickup("BOOK/1", {
      pickupDateTime: "2026-09-20T10:00",
      odometerKm: 12000
    })).resolves.toEqual({ message: "Vehicle picked up successfully." });

    expect(fetchFunction).toHaveBeenCalledWith("/api/v1/rental/BOOK%2F1", expect.objectContaining({
      body: JSON.stringify({ pickupDateTime: "2026-09-20T10:00", odometerKm: 12000 })
    }));
  });

  it("posts the return contract and reads its JSON string response", async () => {
    const fetchFunction = vi.fn<FetchFunction>().mockResolvedValue(
      jsonResponse("Vehicle returned successfully.")
    );
    const client = new RentalService("/api", fetchFunction);

    await expect(client.registerReturn("BOOK-1", {
      returnDateTime: "2026-09-20T11:00",
      odometerKm: 12100
    })).resolves.toEqual({ message: "Vehicle returned successfully." });

    expect(fetchFunction).toHaveBeenCalledWith("/api/v1/rental/BOOK-1/return", expect.objectContaining({
      body: JSON.stringify({ returnDateTime: "2026-09-20T11:00", odometerKm: 12100 })
    }));
  });

  it("uses problem details to report API failures", async () => {
    const fetchFunction = vi.fn<FetchFunction>().mockResolvedValue(jsonResponse({
      title: "Rental conflict",
      detail: "The rental has already been picked up."
    }, 409));
    const client = new RentalService("/api", fetchFunction);

    await expect(client.registerPickup("BOOK-1", {
      pickupDateTime: "2026-09-20T10:00",
      odometerKm: 12000
    })).rejects.toEqual(new ApiProblem("The rental has already been picked up.", 409));
  });

  it("uses validation field errors when problem details have no detail", async () => {
    const fetchFunction = vi.fn<FetchFunction>().mockResolvedValue(jsonResponse({
      title: "One or more validation errors occurred.",
      errors: { CustomerId: ["The CustomerId field is required."] }
    }, 400));
    const client = new RentalService("/api", fetchFunction);

    await expect(client.createBooking({ category: "Truck", customerId: "" }))
      .rejects.toEqual(new ApiProblem("The CustomerId field is required.", 400));
  });
});
