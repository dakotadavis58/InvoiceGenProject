namespace InvoiceGeneratorAPI.DTOs.Company;

public class UpdateCompanyRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? InvoicePrefix { get; set; }
    public string? InvoiceNotes { get; set; }
    public string? PaymentInstructions { get; set; }
}