"use server";

import { Invoice, InvoiceFilters, InvoiceTableState } from "@/types/invoice";

const DUMMY_INVOICES: Invoice[] = [
  {
    id: "1",
    invoiceNumber: "000055",
    companyId: "1",
    clientName: "NTx Ventures",
    clientEmail: "billing@ntx.com",
    issueDate: "2024-04-29",
    dueDate: "2024-05-29",
    subTotal: 1000,
    taxRate: 0.2,
    taxAmount: 200,
    totalAmount: 1200,
    status: "overdue",
    createdAt: "2024-04-29T10:00:00Z",
    updatedAt: "2024-04-29T10:00:00Z",
    items: [
      {
        id: "1",
        invoiceId: "1",
        description: "callrail adgen",
        quantity: 1,
        unitPrice: 1000,
        amount: 1000,
      },
    ],
  },
  {
    id: "2",
    invoiceNumber: "000054",
    companyId: "1",
    clientName: "NTx Ventures",
    clientEmail: "billing@ntx.com",
    issueDate: "2024-04-22",
    dueDate: "2024-05-22",
    subTotal: 1000,
    taxRate: 0.2,
    taxAmount: 200,
    totalAmount: 1200,
    status: "overdue",
    createdAt: "2024-04-22T10:00:00Z",
    updatedAt: "2024-04-22T10:00:00Z",
    items: [
      {
        id: "2",
        invoiceId: "2",
        description: "adgen callrail",
        quantity: 1,
        unitPrice: 1000,
        amount: 1000,
      },
    ],
  },
];

export async function fetchFilteredInvoices(
  filters: InvoiceFilters
): Promise<Invoice[]> {
  // This would be replaced with an actual API call
  return DUMMY_INVOICES.filter((invoice) => {
    // Client name filter
    if (
      filters.clientName &&
      !invoice.clientName
        .toLowerCase()
        .includes(filters.clientName.toLowerCase())
    ) {
      return false;
    }

    // Status filter
    if (filters.status && invoice.status !== filters.status) {
      return false;
    }

    // Date range filter
    if (filters.dateRange) {
      const invoiceDate = new Date(invoice.issueDate);
      if (
        filters.dateRange.start &&
        new Date(filters.dateRange.start) > invoiceDate
      ) {
        return false;
      }
      if (
        filters.dateRange.end &&
        new Date(filters.dateRange.end) < invoiceDate
      ) {
        return false;
      }
    }

    // Keyword search
    if (filters.keyword) {
      const searchText =
        filters.searchField === "number"
          ? invoice.invoiceNumber
          : filters.searchField === "description"
          ? invoice.items.map((item) => item.description).join(" ")
          : `${invoice.invoiceNumber} ${invoice.clientName} ${invoice.items
              .map((item) => item.description)
              .join(" ")}`;

      if (!searchText.toLowerCase().includes(filters.keyword.toLowerCase())) {
        return false;
      }
    }

    return true;
  });
}

export async function fetchInvoicesWithPagination(
  page: number = 1,
  itemsPerPage: number = 30,
  filters: InvoiceFilters = {}
): Promise<{ invoices: Invoice[]; tableState: InvoiceTableState }> {
  const filteredInvoices = await fetchFilteredInvoices(filters);
  const startIndex = (page - 1) * itemsPerPage;

  return {
    invoices: filteredInvoices.slice(startIndex, startIndex + itemsPerPage),
    tableState: {
      currentPage: page,
      itemsPerPage,
      totalPages: Math.ceil(filteredInvoices.length / itemsPerPage),
      totalItems: filteredInvoices.length,
    },
  };
}

export async function fetchInvoiceStats() {
  // This would be an API call to get real-time stats
  const allInvoices = await fetchAllInvoices();

  const calculateTotal = (status: Invoice["status"]) =>
    allInvoices
      .filter((inv) => inv.status === status)
      .reduce((sum, inv) => sum + inv.totalAmount, 0);

  return {
    overdue: `$${calculateTotal("overdue").toLocaleString()}`,
    outstanding: `$${calculateTotal("sent").toLocaleString()}`,
    draft: `$${calculateTotal("draft").toLocaleString()}`,
  };
}

export async function fetchRecentInvoices(): Promise<Invoice[]> {
  // Return most recent 6 invoices
  return DUMMY_INVOICES.slice(0, 6);
}

export async function fetchAllInvoices(): Promise<Invoice[]> {
  return DUMMY_INVOICES;
}
