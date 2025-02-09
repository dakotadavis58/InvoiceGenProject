namespace InvoiceGeneratorAPI.Models;

public class Client
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }  // Clients belong to a company
    public Company Company { get; set; } = null!;

    // Basic Info
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    // Additional Info
    public string? TaxNumber { get; set; }  // VAT number or tax ID
    public string? Notes { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public List<Invoice> Invoices { get; set; } = new();
}