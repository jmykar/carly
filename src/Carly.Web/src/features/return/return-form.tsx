import { useEffect, useState, type FormEvent } from "react";
import { getProblemMessage } from "../../services/schema-helper";
import { type RentalClient } from "../../services/schema";
import { FeedbackMessage } from "../../components/feedback-message";

interface ReturnFormProps {
  client: RentalClient;
  suggestedBookingNumber?: string;
}

export function ReturnForm({ client, suggestedBookingNumber }: ReturnFormProps) {
  const [bookingNumber, setBookingNumber] = useState(suggestedBookingNumber ?? "");
  const [returnDateTime, setReturnDateTime] = useState("");
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
    if (bookingNumber.trim() === "" || returnDateTime === "") {
      setError("Supply the booking number and return date/time.");
      return;
    }

    if (!Number.isInteger(odometer) || odometer < 0) {
      setError("Supply a non-negative whole-kilometre odometer reading.");
      return;
    }

    setIsSubmitting(true);
    try {
      const result = await client.registerReturn(bookingNumber.trim(), {
        returnDateTime,
        odometerKm: odometer
      });
      setMessage(result.message);
    } catch (requestError) {
      setError(getProblemMessage(requestError));
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form className="space-y-5" onSubmit={handleSubmit} noValidate>
      <div>
        <label className="block text-sm font-medium text-slate-800" htmlFor="return-booking-number">
          Booking number
        </label>
        <input
          className="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 shadow-sm focus:border-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-200"
          id="return-booking-number"
          onChange={(event) => setBookingNumber(event.target.value)}
          value={bookingNumber}
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-800" htmlFor="return-date-time">
          Return date and time
        </label>
        <input
          className="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 shadow-sm focus:border-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-200"
          id="return-date-time"
          onChange={(event) => setReturnDateTime(event.target.value)}
          type="datetime-local"
          value={returnDateTime}
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-800" htmlFor="return-odometer">
          Odometer reading (km)
        </label>
        <input
          className="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 shadow-sm focus:border-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-200"
          id="return-odometer"
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
        {isSubmitting ? "Registering return…" : "Register return"}
      </button>

      <FeedbackMessage error={error} message={message} />
    </form>
  );
}
