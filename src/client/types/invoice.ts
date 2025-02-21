export type InvoiceStatus = "draft" | "sent" | "paid" | "overdue" | "cancelled";

export interface Invoice {
  id: string;
  invoiceNumber: string;
  companyId: string;

  // Client details
  clientId?: string;
  clientName: string;
  clientEmail: string;
  clientAddress?: string;

  // Invoice details
  issueDate: string;
  dueDate: string;
  subTotal: number;
  taxRate: number;
  taxAmount: number;
  totalAmount: number;
  status: InvoiceStatus;
  notes?: string;

  // Payment details
  paymentTerms?: string;
  paymentInstructions?: string;

  // Audit
  createdAt: string;
  updatedAt: string;

  // Items
  items: InvoiceItem[];
}

export interface InvoiceItem {
  id: string;
  invoiceId: string;
  description: string;
  quantity: number;
  unitPrice: number;
  amount: number;
}

export interface InvoiceTableState {
  currentPage: number;
  itemsPerPage: number;
  totalPages: number;
  totalItems: number;
}

export interface InvoiceFilters {
  clientName?: string;
  status?: InvoiceStatus;
  dateRange?:
    | {
        start?: string;
        end?: string;
      }
    | undefined;
  keyword?: string;
  searchField?: "all" | "number" | "description";
}
