import { StatusBadge } from "@/components/ui/status-badge";
import { Invoice, InvoiceStatus, InvoiceTableState } from "@/types/invoice";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  DoubleArrowLeftIcon,
  DoubleArrowRightIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
} from "@radix-ui/react-icons";

interface InvoiceTableProps {
  invoices: Invoice[];
  tableState: InvoiceTableState;
  onPageChange: (page: number) => void;
  onItemsPerPageChange: (items: number) => void;
}

export const InvoiceTable = ({
  invoices,
  tableState,
  onPageChange,
  onItemsPerPageChange,
}: InvoiceTableProps) => {
  const { currentPage, totalPages, itemsPerPage } = tableState;

  return (
    <div className="border rounded-lg">
      <table className="w-full">
        <thead>
          <tr className="border-b">
            <th className="text-left py-4 px-6">Client/Invoice Number</th>
            <th className="text-left py-4 px-6">Description</th>
            <th className="text-left py-4 px-6">Issued Date/Due Date</th>
            <th className="text-right py-4 px-6">Amount/Status</th>
          </tr>
        </thead>
        <tbody>
          {invoices.map((invoice) => (
            <tr key={invoice.id} className="border-b">
              <td className="py-4 px-6">
                <div>{invoice.clientName}</div>
                <div className="text-sm text-gray-600">
                  {invoice.invoiceNumber}
                </div>
              </td>
              <td className="px-6">
                {invoice.items[0]?.description || "No description"}
              </td>
              <td className="px-6">
                <div>{new Date(invoice.issueDate).toLocaleDateString()}</div>
                <div className="text-sm text-gray-600">
                  Due {new Date(invoice.dueDate).toLocaleDateString()}
                </div>
              </td>
              <td className="text-right px-6">
                <div>${invoice.totalAmount.toLocaleString()}</div>
                <div className="mt-1">
                  <StatusBadge
                    status={invoice.status.toLowerCase() as InvoiceStatus}
                  />
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Table Footer */}
      <div className="border-t px-6 py-4 bg-gray-50 rounded-b-lg">
        <div className="flex justify-between items-center">
          {/* Pagination Controls */}
          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="icon"
              disabled={currentPage === 1}
              onClick={() => onPageChange(1)}
            >
              <DoubleArrowLeftIcon className="h-4 w-4" />
            </Button>
            <Button
              variant="ghost"
              size="icon"
              disabled={currentPage === 1}
              onClick={() => onPageChange(currentPage - 1)}
            >
              <ChevronLeftIcon className="h-4 w-4" />
            </Button>
            <span className="text-sm">
              Page {currentPage} of {totalPages}
            </span>
            <Button
              variant="ghost"
              size="icon"
              disabled={currentPage === totalPages}
              onClick={() => onPageChange(currentPage + 1)}
            >
              <ChevronRightIcon className="h-4 w-4" />
            </Button>
            <Button
              variant="ghost"
              size="icon"
              disabled={currentPage === totalPages}
              onClick={() => onPageChange(totalPages)}
            >
              <DoubleArrowRightIcon className="h-4 w-4" />
            </Button>
          </div>

          {/* Items per page */}
          <div className="flex items-center gap-4">
            <div className="flex items-center gap-2">
              <span className="text-sm">Items per page:</span>
              <Select
                value={itemsPerPage.toString()}
                onValueChange={(value) => onItemsPerPageChange(parseInt(value))}
              >
                <SelectTrigger className="w-[70px]">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="10">10</SelectItem>
                  <SelectItem value="20">20</SelectItem>
                  <SelectItem value="30">30</SelectItem>
                  <SelectItem value="50">50</SelectItem>
                </SelectContent>
              </Select>
            </div>

            {/* Grand Total */}
            <div className="flex items-center gap-2">
              <span className="text-sm font-medium">Grand Total:</span>
              <span className="text-sm">
                $
                {invoices
                  .reduce((sum, inv) => sum + inv.totalAmount, 0)
                  .toLocaleString()}{" "}
                USD
              </span>
            </div>

            {/* Archive Links */}
            <div className="flex items-center gap-2">
              <Button variant="link" className="text-sm">
                View Archived Invoices
              </Button>
              <span className="text-sm text-gray-500">or</span>
              <Button variant="link" className="text-sm">
                deleted
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
