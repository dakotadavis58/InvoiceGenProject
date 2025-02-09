export interface Company {
  id: string;
  name: string;
  logoUrl?: string;
  website?: string;
  phone?: string;
  email: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
  taxNumber?: string;
  registrationNumber?: string;
  invoicePrefix?: string;
  invoiceNotes?: string;
  paymentInstructions?: string;
}

export interface CreateCompanyRequest {
  name: string;
  email: string;
  website?: string;
  phone?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
  taxNumber?: string;
  registrationNumber?: string;
  invoicePrefix?: string;
  invoiceNotes?: string;
  paymentInstructions?: string;
}

export type UpdateCompanyRequest = Partial<CreateCompanyRequest>;
