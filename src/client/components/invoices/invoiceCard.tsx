import { Invoice } from "@/types/invoice";

import { Card, CardContent, CardHeader } from "../ui/card";
import { StatusBadge } from "../ui/status-badge";

export interface InvoiceCardProps {
  invoiceNumber: string;
  clientName: string;
  issueDate: string;
  totalAmount: number;
  status: Invoice["status"];
}

export const InvoiceCard = ({
  invoiceNumber,
  clientName,
  issueDate,
  totalAmount,
  status,
}: InvoiceCardProps) => {
  return (
    <Card>
      <CardHeader>
        <div className="text-sm text-gray-600">{invoiceNumber}</div>
        <div className="font-medium">{clientName}</div>
        <div className="text-sm text-gray-600">
          {new Date(issueDate).toLocaleDateString()}
        </div>
      </CardHeader>
      <CardContent>
        <div className="text-lg font-semibold">
          ${totalAmount.toLocaleString()}
        </div>
        <div className="mt-2">
          {/* eslint-disable-next-line @typescript-eslint/no-explicit-any */}
          <StatusBadge status={status.toLowerCase() as any} />
        </div>
      </CardContent>
    </Card>
  );
};
