// /lib/actions/invoice.ts
"use server";

import { CreateInvoiceRequest } from "@/types/invoice";

export async function createInvoice(data: CreateInvoiceRequest) {
  // Server-side validation
  // API call to our backend
  // Return result
}

export async function generateInvoiceNumber() {
  // Fetch next invoice number from the backend
}
