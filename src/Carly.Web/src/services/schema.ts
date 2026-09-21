export const carCategories = ["SmallCar", "Combi", "Truck"] as const;

export type CarCategory = (typeof carCategories)[number];

export interface CreateBookingRequest {
  category: CarCategory;
  customerId: string;
}

export interface BookingResponse {
  bookingNumber: string;
  registrationNumber: string;
  category: CarCategory;
}

export interface PickupRequest {
  pickupDateTime: string;
  odometerKm: number;
}

export interface ReturnRequest {
  returnDateTime: string;
  odometerKm: number;
}

export interface OperationResult {
  message: string;
}

export interface RentalClient {
  createBooking(request: CreateBookingRequest): Promise<BookingResponse>;
  registerPickup(bookingNumber: string, request: PickupRequest): Promise<OperationResult>;
  registerReturn(bookingNumber: string, request: ReturnRequest): Promise<OperationResult>;
}