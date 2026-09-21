import { useEffect, useState, type FormEvent } from "react";
import { getProblemMessage } from "../../services/schema-helper";
import { type RentalClient } from "../../services/schema";
import { FeedbackMessage } from "../../components/feedback-message";

interface PickupFormProps {
  client: RentalClient;
  suggestedBookingNumber?: string;
  onRegistered: (bookingNumber: string) => void;
}

export function PickupForm({ client, suggestedBookingNumber, onRegistered }: PickupFormProps) {
  const [bookingNumber, setBookingNumber] = useState(suggestedBookingNumber ?? "");
  const [pickupDateTime, setPickupDateTime] = useState("");
  const [odometerKm, setOdometerKm] = useState("");
  const [error, setError] = useState<string>();
  const [message, setMessage] = useState<string>();
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (suggestedBookingNumber !== undefined) {
      setBookingNumber(suggestedBookingNumber);
    }
  }, [suggestedBookingNumber]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(undefined);
    setMessage(undefined);

    const odometer = Number(odometerKm);
    if (bookingNumber.trim() === "" || pickupDateTime === "") {
      setError("Supply the booking number and pickup date/time.");
      return;
    }

    if (!Number.isInteger(odometer) || odometer < 0) {
      setError("Supply a non-negative whole-kilometre odometer reading.");
      return;
    }

    setIsSubmitting(true);
    try {
      const result = await client.registerPickup(bookingNumber.trim(), {
        pickupDateTime,
        odometerKm: odometer
      });
      setMessage(result.message);
      onRegistered(bookingNumber.trim());
    } catch (requestError) {
      setError(getProblemMessage(requestError));
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form className="space-y-5" onSubmit={handleSubmit} noValidate>
      <div>
        <label className="block text-sm font-medium text-slate-800" htmlFor="pickup-booking-number">
          Booking number
        </label>
        <input
          className="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 shadow-sm focus:border-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-200"
          id="pickup-booking-number"
          onChange={(event) => setBookingNumber(event.target.value)}
          value={bookingNumber}
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-800" htmlFor="pickup-date-time">
          Pickup date and time
        </label>
        <input
          className="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 shadow-sm focus:border-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-200"
          id="pickup-date-time"
          onChange={(event) => setPickupDateTime(event.target.value)}
          type="datetime-local"
          value={pickupDateTime}
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-800" htmlFor="pickup-odometer">
          Odometer reading (km)
        </label>
        <input
          className="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 shadow-sm focus:border-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-200"
          id="pickup-odometer"
          min="0"
          onChange={(event) => setOdometerKm(event.target.value)}
          step="1"
          type="number"
          value={odometerKm}
        />
      </div>

      <button
        className="rounded-md bg-blue-700 px-4 py-2 font-semibold text-white hover:bg-blue-800 disabled:cursor-not-allowed disabled:bg-slate-400"
        disabled={isSubmitting}
        type="submit"
      >
        {isSubmitting ? "Registering pickup…" : "Register pickup"}
      </button>

      <FeedbackMessage error={error} message={message} />
    </form>
  );
}
