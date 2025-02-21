import { StatCard } from "@/components/ui/stat-card";
import {
  fetchAllInvoices,
  fetchInvoiceStats,
  fetchRecentInvoices,
} from "./actions";
import { NewInvoiceButton, SearchSection } from "./invoices-client";
import { InvoiceCard } from "@/components/invoices/invoiceCard";

// This is now a Server Component
const InvoicesPage = async () => {
  // Fetch data server-side
  const recentInvoices = await fetchRecentInvoices();
  const allInvoices = await fetchAllInvoices();
  const stats = await fetchInvoiceStats();

  return (
    <div className="p-8">
      {/* Header - Static */}
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-2xl font-bold">Invoices</h1>
        <NewInvoiceButton />
      </div>

      {/* Stats - Server Rendered */}
      <div className="grid grid-cols-3 gap-6 mb-8">
        <StatCard amount={stats.overdue} label="overdue" />
        <StatCard amount={stats.outstanding} label="total outstanding" />
        <StatCard amount={stats.draft} label="in draft" />
      </div>

      {/* Recently Updated - Server Rendered */}
      <div className="mb-8">
        <h2 className="text-xl font-semibold mb-4">Recently Updated</h2>
        <div className="grid grid-cols-6 gap-4">
          {recentInvoices.map((invoice) => (
            <InvoiceCard
              key={invoice.id}
              invoiceNumber={invoice.invoiceNumber}
              clientName={invoice.clientName}
              issueDate={invoice.issueDate}
              totalAmount={invoice.totalAmount}
              status={invoice.status}
            />
          ))}
        </div>
      </div>

      {/* All Invoices Section - Client interactions needed */}
      <SearchSection initialInvoices={allInvoices} />
    </div>
  );
};

export default InvoicesPage;
