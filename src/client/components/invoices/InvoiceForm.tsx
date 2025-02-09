"use client";

import { useState } from "react";
import { createInvoice } from "@/app/actions/invoice";
import { Company } from "@/types/company";

interface InvoiceFormProps {
  companyData: Company;
}

interface InvoiceItem {
  description: string;
  quantity: number;
  unitPrice: number;
  amount: number;
}

interface InvoiceFormData {
  clientName: string;
  clientEmail: string;
  clientAddress: string;
  issueDate: Date;
  dueDate: Date;
  items: InvoiceItem[];
  notes: string;
  paymentTerms: string;
  paymentInstructions: string;
  taxRate: number;
}

export function InvoiceForm({ companyData }: InvoiceFormProps) {
  const [formData, setFormData] = useState<InvoiceFormData>({
    clientName: "",
    clientEmail: "",
    clientAddress: "",
    issueDate: new Date(),
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000), // 30 days from now
    items: [],
    notes: "",
    paymentTerms: "",
    paymentInstructions: "",
    taxRate: 0,
  });

  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setIsSubmitting(true);

    try {
      await createInvoice({
        ...formData,
        companyId: companyData.id,
      });
      // Handle successful creation (e.g., redirect to invoice list)
    } catch (error) {
      // Handle error
      console.error("Failed to create invoice:", error);
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="grid grid-cols-2 gap-8">
      {/* Left side - Form inputs */}
      <form onSubmit={handleSubmit} className="space-y-6">
        <div className="bg-white p-6 rounded-lg shadow">
          {/* Company Details (read-only) */}
          <div className="mb-6">
            <h2 className="text-lg font-semibold mb-4">Your Company</h2>
            <div className="text-sm text-gray-600">
              <p>{companyData.name}</p>
              <p>{companyData.addressLine1}</p>
              <p>{companyData.addressLine2}</p>
              <p>{companyData.city}</p>
              <p>{companyData.state}</p>
              <p>{companyData.postalCode}</p>
              <p>{companyData.country}</p>
              <p>{companyData.email}</p>
              {companyData.phone && <p>{companyData.phone}</p>}
              {companyData.taxNumber && (
                <p>Tax Number: {companyData.taxNumber}</p>
              )}
            </div>
          </div>

          {/* Client Details */}
          <div className="mb-6">
            <h2 className="text-lg font-semibold mb-4">Client Information</h2>
            {/* Add client input fields here */}
          </div>

          {/* Invoice Items */}
          <div className="mb-6">
            <h2 className="text-lg font-semibold mb-4">Invoice Items</h2>
            {/* Add invoice items component here */}
          </div>

          {/* Additional Details */}
          <div className="mb-6">
            <h2 className="text-lg font-semibold mb-4">Additional Details</h2>
            {/* Add notes, payment terms, etc. here */}
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 disabled:bg-blue-300"
          >
            {isSubmitting ? "Creating Invoice..." : "Create Invoice"}
          </button>
        </div>
      </form>

      {/* Right side - Preview */}
      <div className="bg-white p-6 rounded-lg shadow">
        <h2 className="text-lg font-semibold mb-4">Preview</h2>
        {/* Add invoice preview component here */}
      </div>
    </div>
  );
}
