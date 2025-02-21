"use client";

import * as React from "react";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { InvoiceFilters, InvoiceStatus } from "@/types/invoice";

interface AdvancedSearchProps {
  onClose: () => void;
  onApply: (filters: InvoiceFilters) => void;
}

export const AdvancedSearch = ({ onClose, onApply }: AdvancedSearchProps) => {
  const [filters, setFilters] = React.useState<InvoiceFilters>({
    clientName: "",
    status: undefined,
    dateRange: {
      start: undefined,
      end: undefined,
    },
    keyword: "",
    searchField: "all",
  });

  const handleReset = () => {
    setFilters({
      clientName: "",
      status: undefined,
      dateRange: {
        start: undefined,
        end: undefined,
      },
      keyword: "",
      searchField: "all",
    });
  };

  const handleDateChange = (field: "start" | "end", value: string) => {
    setFilters((prev) => ({
      ...prev,
      dateRange: {
        ...prev.dateRange,
        [field]: value || undefined,
      } as InvoiceFilters["dateRange"],
    }));
  };

  return (
    <div className="p-6 border rounded-lg bg-white shadow-lg">
      <div className="grid gap-6">
        {/* Client Selection */}
        <div>
          <label className="text-sm font-medium mb-2 block">Client</label>
          <Input
            placeholder="Search clients..."
            value={filters.clientName}
            onChange={(e) =>
              setFilters((prev) => ({ ...prev, clientName: e.target.value }))
            }
          />
        </div>

        {/* Status */}
        <div>
          <label className="text-sm font-medium mb-2 block">Status</label>
          <Select
            value={filters.status}
            onValueChange={(value) =>
              setFilters((prev) => ({
                ...prev,
                status: value as InvoiceStatus,
              }))
            }
          >
            <SelectTrigger>
              <SelectValue placeholder="Select status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="draft">Draft</SelectItem>
              <SelectItem value="sent">Sent</SelectItem>
              <SelectItem value="paid">Paid</SelectItem>
              <SelectItem value="overdue">Overdue</SelectItem>
              <SelectItem value="cancelled">Cancelled</SelectItem>
            </SelectContent>
          </Select>
        </div>

        {/* Date Range */}
        <div>
          <label className="text-sm font-medium mb-2 block">Date Range</label>
          <div className="grid grid-cols-2 gap-4">
            <Input
              type="date"
              value={filters.dateRange?.start || ""}
              onChange={(e) => handleDateChange("start", e.target.value)}
            />
            <Input
              type="date"
              value={filters.dateRange?.end || ""}
              onChange={(e) => handleDateChange("end", e.target.value)}
            />
          </div>
        </div>

        {/* Search Field */}
        <div>
          <label className="text-sm font-medium mb-2 block">Search In</label>
          <Select
            value={filters.searchField}
            onValueChange={(value) =>
              setFilters((prev) => ({
                ...prev,
                searchField: value as "all" | "number" | "description",
              }))
            }
          >
            <SelectTrigger>
              <SelectValue placeholder="Select field" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Fields</SelectItem>
              <SelectItem value="number">Invoice Number</SelectItem>
              <SelectItem value="description">Description</SelectItem>
            </SelectContent>
          </Select>
        </div>

        {/* Keyword Search */}
        <div>
          <label className="text-sm font-medium mb-2 block">Keyword</label>
          <Input
            placeholder="Search..."
            value={filters.keyword}
            onChange={(e) =>
              setFilters((prev) => ({ ...prev, keyword: e.target.value }))
            }
          />
        </div>

        {/* Actions */}
        <div className="flex justify-between items-center pt-4 border-t">
          <Button
            variant="link"
            className="text-blue-600"
            onClick={handleReset}
          >
            Reset all
          </Button>
          <div className="flex gap-2">
            <Button variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button onClick={() => onApply(filters)} variant="default">
              Apply
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};
