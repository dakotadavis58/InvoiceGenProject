"use client";

import React from "react";
import { PlusIcon } from "@radix-ui/react-icons";
import { Button } from "@/components/ui/button";
import { AdvancedSearch } from "@/components/invoices/advanced-search";
import { InvoiceTable } from "@/components/invoices/InvoiceTable";
import { fetchInvoicesWithPagination } from "./actions";
import { Invoice, InvoiceFilters } from "@/types/invoice";

export const NewInvoiceButton = () => (
  <Button variant="success">New Invoice</Button>
);

export const SearchSection = ({
  initialInvoices,
}: {
  initialInvoices: Invoice[];
}) => {
  const [showAdvancedSearch, setShowAdvancedSearch] = React.useState(false);
  const [invoices, setInvoices] = React.useState(initialInvoices);
  const [searchQuery, setSearchQuery] = React.useState("");
  const [tableState, setTableState] = React.useState({
    currentPage: 1,
    itemsPerPage: 30,
    totalPages: Math.ceil(initialInvoices.length / 30),
    totalItems: initialInvoices.length,
  });

  const handlePageChange = async (page: number) => {
    const result = await fetchInvoicesWithPagination(
      page,
      tableState.itemsPerPage
    );
    setInvoices(result.invoices);
    setTableState(result.tableState);
  };

  const handleItemsPerPageChange = async (items: number) => {
    const result = await fetchInvoicesWithPagination(1, items);
    setInvoices(result.invoices);
    setTableState(result.tableState);
  };

  const handleApplyFilters = async (filters: InvoiceFilters) => {
    const result = await fetchInvoicesWithPagination(
      1,
      tableState.itemsPerPage,
      filters
    );
    setInvoices(result.invoices);
    setTableState(result.tableState);
    setShowAdvancedSearch(false);
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <div className="flex items-center gap-2">
          <h2 className="text-xl font-semibold">All Invoices</h2>
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8 rounded-full bg-green-600 hover:bg-green-700"
          >
            <PlusIcon className="h-4 w-4 text-white" />
          </Button>
        </div>
        <div className="flex gap-2">
          <input
            type="text"
            placeholder="Search"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="px-4 py-2 border rounded-lg"
          />
          <Button
            variant="outline"
            onClick={() => setShowAdvancedSearch(!showAdvancedSearch)}
          >
            Advanced Search
          </Button>
        </div>
      </div>

      {showAdvancedSearch && (
        <div className="mb-6">
          <AdvancedSearch
            onClose={() => setShowAdvancedSearch(false)}
            onApply={handleApplyFilters}
          />
        </div>
      )}

      <InvoiceTable
        invoices={invoices}
        tableState={tableState}
        onPageChange={handlePageChange}
        onItemsPerPageChange={handleItemsPerPageChange}
      />
    </div>
  );
};
