import { useState } from "react";
import type { BookingResponse, RentalClient } from "./services/schema";
import { RentalService } from "./services/rental-service";
import { Tabs, type TabId } from "./components/tabs";
import { BookingForm } from "./features/booking/booking-form";
import { PickupForm } from "./features/pickup/pickup-form";
import { ReturnForm } from "./features/return/return-form";

const defaultClient = new RentalService();

interface AppProps {
  client?: RentalClient;
}

export default function App({ client = defaultClient }: AppProps) {
  const [activeTab, setActiveTab] = useState<TabId>("booking");
  const [booking, setBooking] = useState<BookingResponse>();
  const [pickupBookingNumber, setPickupBookingNumber] = useState<string>();

  function handleBookingCreated(createdBooking: BookingResponse) {
    setBooking(createdBooking);
    setPickupBookingNumber(undefined);
  }

  function handlePickupRegistered(bookingNumber: string) {
    setPickupBookingNumber(bookingNumber);
  }

  return (
    <main className="mx-auto min-h-screen max-w-3xl px-4 py-10 sm:px-6">
      <header className="mb-8">
        <p className="text-sm font-semibold uppercase tracking-wide text-blue-700">Carly</p>
        <h1 className="mt-2 text-3xl font-bold tracking-tight text-slate-950">Rental lifecycle</h1>
        <p className="mt-2 max-w-2xl text-slate-600">
          Create a test booking, register vehicle pickup, and register the return.
        </p>
      </header>

      <Tabs activeTab={activeTab} onChange={setActiveTab} />

      <section
        aria-labelledby={`${activeTab}-tab`}
        className="rounded-b-lg bg-white p-6 shadow-sm ring-1 ring-slate-200"
        id={`${activeTab}-panel`}
        role="tabpanel"
      >
        {activeTab === "booking" && (
          <>
            <h2 className="text-xl font-semibold text-slate-950">Create test booking</h2>
            <p className="mt-1 text-sm text-slate-600">
              This creates the minimum booking needed to test the rental lifecycle.
            </p>
            <div className="mt-6">
              <BookingForm client={client} onCreated={handleBookingCreated} />
            </div>
          </>
        )}
        {activeTab === "pickup" && (
          <>
            <h2 className="text-xl font-semibold text-slate-950">Register vehicle pickup</h2>
            <p className="mt-1 text-sm text-slate-600">Record when the assigned vehicle leaves.</p>
            <div className="mt-6">
              <PickupForm
                client={client}
                onRegistered={handlePickupRegistered}
                suggestedBookingNumber={booking?.bookingNumber}
              />
            </div>
          </>
        )}
        {activeTab === "return" && (
          <>
            <h2 className="text-xl font-semibold text-slate-950">Register vehicle return</h2>
            <p className="mt-1 text-sm text-slate-600">Record the return odometer reading.</p>
            <div className="mt-6">
              <ReturnForm client={client} suggestedBookingNumber={pickupBookingNumber} />
            </div>
          </>
        )}
      </section>
    </main>
  );
}
