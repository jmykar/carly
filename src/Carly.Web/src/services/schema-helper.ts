import type {
  BookingResponse,
  CreateBookingRequest,
  OperationResult,
  PickupRequest,
  ReturnRequest
} from "./schema";

export interface RentalClient {
  createBooking(request: CreateBookingRequest): Promise<BookingResponse>;
  registerPickup(bookingNumber: string, request: PickupRequest): Promise<OperationResult>;
  registerReturn(bookingNumber: string, request: ReturnRequest): Promise<OperationResult>;
}

export class ApiProblem extends Error {
  public constructor(
    message: string,
    public readonly status: number
  ) {
    super(message);
  }
}

export function getProblemMessage(error: unknown): string {
  return error instanceof Error ? error.message : "The request could not be completed.";
}
