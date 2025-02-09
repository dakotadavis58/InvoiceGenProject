import { getCompany } from "@/app/actions/company";
import { InvoiceForm } from "@/components/invoices/InvoiceForm";

async function NewInvoicePage() {
  // Fetch initial data server-side
  const company = await getCompany();

  return (
    <div className="container mx-auto py-8">
      <h1 className="text-2xl font-bold mb-6">Create New Invoice</h1>
      <InvoiceForm companyData={company} />
    </div>
  );
}

export default NewInvoicePage;
