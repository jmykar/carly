import { useState, type FormEvent } from "react";
import { getProblemMessage } from "../../services/schema-helper";
import { type RentalClient } from "../../services/schema";
import { carCategories, type BookingResponse, type CarCategory } from "../../services/schema";
import { FeedbackMessage } from "../../components/feedback-message";

interface BookingFormProps {
  client: RentalClient;
  onCreated: (booking: BookingResponse) => void;
}

export function BookingForm({ client, onCreated }: BookingFormProps) {
  const [category, setCategory] = useState<CarCategory | "">("");
  const [customerId, setCustomerId] = useState("");
  const [error, setError] = useState<string>();
  const [booking, setBooking] = useState<BookingResponse>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const customerIdPattern = /^\d{6}-\d{4}$/;

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(undefined);
    setBooking(undefined);

    const trimmedCustomerId = customerId.trim();
    if (category === "" || trimmedCustomerId === "") {
      setError("Supply a customer identifier and select a car category.");
      return;
    }

    if (!customerIdPattern.test(trimmedCustomerId)) {
      setError("Customer identifier must use the format XXXXXX-XXXX, with digits only.");
      return;
    }

    setIsSubmitting(true);
    try {
      const createdBooking = await client.createBooking({
        category,
        customerId: trimmedCustomerId
      });
      setBooking(createdBooking);
      onCreated(createdBooking);
    } catch (requestError) {
      setError(getProblemMessage(requestError));
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form className="space-y-5" onSubmit={handleSubmit} noValidate>
      <div>
        <label className="block text-sm font-medium text-slate-800" htmlFor="booking-customer-id">
          Customer identifier
        </label>
        <input
          className="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 shadow-sm focus:border-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-200"
          id="booking-customer-id"
          maxLength={11}
          onChange={(event) => setCustomerId(event.target.value)}
          pattern="[0-9]{6}-[0-9]{4}"
          value={customerId}
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-800" htmlFor="booking-category">
          Car category
        </label>
        <select
          className="mt-1 block w-full rounded-md border border-slate-300 bg-white px-3 py-2 shadow-sm focus:border-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-200"
          id="booking-category"
          onChange={(event) => setCategory(event.target.value as CarCategory | "")}
          value={category}
        >
          <option value="">Select a category</option>
          {carCategories.map((option) => (
            <option key={option} value={option}>
              {option}
            </option>
          ))}
        </select>
      </div>

      <button
        className="rounded-md bg-blue-700 px-4 py-2 font-semibold text-white hover:bg-blue-800 disabled:cursor-not-allowed disabled:bg-slate-400"
        disabled={isSubmitting}
        type="submit"
      >
        {isSubmitting ? "Creating booking…" : "Create test booking"}
      </button>

      <FeedbackMessage error={error} />
      {booking !== undefined && (
        <section aria-label="Created booking" className="rounded-md bg-emerald-50 p-4 text-sm text-emerald-900">
          <p className="font-semibold">Test booking created</p>
          <dl className="mt-2 grid gap-1 sm:grid-cols-2">
            <div>
              <dt className="font-medium">Booking number</dt>
              <dd>{booking.bookingNumber}</dd>
            </div>
            <div>
              <dt className="font-medium">Registration number</dt>
              <dd>{booking.registrationNumber}</dd>
            </div>
          </dl>
        </section>
      )}
    </form>
  );
}
