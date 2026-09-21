export type TabId = "booking" | "pickup" | "return";

interface Tab {
  id: TabId;
  label: string;
}

const tabs: Tab[] = [
  { id: "booking", label: "Booking" },
  { id: "pickup", label: "Vehicle pickup" },
  { id: "return", label: "Return" }
];

interface TabsProps {
  activeTab: TabId;
  onChange: (tab: TabId) => void;
}

export function Tabs({ activeTab, onChange }: TabsProps) {
  return (
    <div aria-label="Rental lifecycle" className="flex gap-1 border-b border-slate-200" role="tablist">
      {tabs.map((tab) => (
        <button
          aria-controls={`${tab.id}-panel`}
          aria-selected={activeTab === tab.id}
          className={`rounded-t-md px-4 py-3 text-sm font-semibold focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-700 ${
            activeTab === tab.id
              ? "bg-blue-700 text-white"
              : "text-slate-700 hover:bg-slate-100"
          }`}
          id={`${tab.id}-tab`}
          key={tab.id}
          onClick={() => onChange(tab.id)}
          role="tab"
          type="button"
        >
          {tab.label}
        </button>
      ))}
    </div>
  );
}
