import { ApiProblem } from "./schema-helper";
import type { RentalClient } from "./schema";

import type {
  BookingResponse,
  CreateBookingRequest,
  OperationResult,
  PickupRequest,
  ReturnRequest
} from "./schema";

export type FetchFunction = (input: RequestInfo | URL, init?: RequestInit) => Promise<Response>;

export class RentalService implements RentalClient {
  public constructor(
    private readonly baseUrl = getApiBaseUrl(),
    private readonly fetchFunction: FetchFunction = globalThis.fetch.bind(globalThis)
  ) {}

  public async createBooking(request: CreateBookingRequest): Promise<BookingResponse> {
    return this.postJson("/v1/booking", request) as Promise<BookingResponse>;
  }

  public async registerPickup(
    bookingNumber: string,
    request: PickupRequest
  ): Promise<OperationResult> {
    const message = await this.postString(
      `/v1/rental/${encodeURIComponent(bookingNumber)}`,
      request
    );
    return { message };
  }

  public async registerReturn(
    bookingNumber: string,
    request: ReturnRequest
  ): Promise<OperationResult> {
    const message = await this.postString(
      `/v1/rental/${encodeURIComponent(bookingNumber)}/return`,
      request
    );
    return { message };
  }

  private async postJson(path: string, body: object): Promise<unknown> {
    const response = await this.post(path, body);
    return response.json();
  }

  private async postString(path: string, body: object): Promise<string> {
    const response = await this.post(path, body);
    const text = await response.text();

    try {
      const value: unknown = JSON.parse(text);
      if (typeof value === "string") {
        return value;
      }
    } catch {
      return text;
    }

    throw new Error("The API returned an invalid success response.");
  }

  private async post(path: string, body: object): Promise<Response> {
    const response = await this.fetchFunction(`${this.baseUrl}${path}`, {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      body: JSON.stringify(body)
    });

    if (!response.ok) {
      throw await toApiProblem(response);
    }

    return response;
  }
}

function getApiBaseUrl(): string {
  const configuredBaseUrl = import.meta.env.VITE_CARLY_API_BASE_URL?.trim();
  return (configuredBaseUrl === "" || configuredBaseUrl === undefined ? "/api" : configuredBaseUrl)
    .replace(/\/+$/, "");
}

async function toApiProblem(response: Response): Promise<ApiProblem> {
  const fallbackMessage = `The request failed with status ${response.status}.`;
  const payload = await response.json().catch(() => undefined);

  if (typeof payload !== "object" || payload === null) {
    return new ApiProblem(fallbackMessage, response.status);
  }

  const problem = payload as {
    detail?: unknown;
    errors?: Record<string, unknown>;
    title?: unknown;
  };
  const fieldError = problem.errors === undefined
    ? undefined
    : Object.values(problem.errors)
      .flatMap((value) => Array.isArray(value) ? value : [])
      .find((value): value is string => typeof value === "string");
  const message = typeof problem.detail === "string"
    ? problem.detail
    : fieldError ?? (typeof problem.title === "string" ? problem.title : fallbackMessage);

  return new ApiProblem(message, response.status);
}
