import { InvoiceStatus } from "@/types/invoice";
import { cn } from "@/lib/utils";

interface StatusBadgeProps {
  status: InvoiceStatus;
}

const statusStyles: Record<InvoiceStatus, string> = {
  draft: "bg-gray-100 text-gray-800",
  sent: "bg-blue-100 text-blue-800",
  paid: "bg-green-100 text-green-800",
  overdue: "bg-red-100 text-red-800",
  cancelled: "bg-gray-100 text-gray-800",
};

export const StatusBadge = ({ status }: StatusBadgeProps) => {
  return (
    <span
      className={cn(
        "px-2 py-1 text-xs font-medium rounded-full",
        statusStyles[status.toLowerCase() as InvoiceStatus]
      )}
    >
      {status.charAt(0).toUpperCase() + status.slice(1)}
    </span>
  );
};
