export interface CreateInvoiceRequest {
  companyId: string;
  customerId: string;
  invoiceDate: string;
  dueDate: string;
  status: string;
}
