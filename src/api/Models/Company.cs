using InvoiceGeneratorAPI.Models;

public class Company
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }  // One-to-one with User
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }  // Business email (can be different from user email)

    // Address information
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    // Business information
    public string? TaxNumber { get; set; }  // VAT/GST/Tax ID
    public string? RegistrationNumber { get; set; }  // Company registration number

    // Invoice customization
    public string? InvoicePrefix { get; set; }  // e.g., "INV-" for INV-0001
    public string? InvoiceNotes { get; set; }  // Default notes/terms
    public string? PaymentInstructions { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}