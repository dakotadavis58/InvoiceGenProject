namespace InvoiceGeneratorAPI.Models;

public class Invoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;

    // Company relationship
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    // Client relationship (optional, can use client details directly)
    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }

    // Client details (used when no stored client or for historical record)
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public string? ClientAddress { get; set; }

    // Invoice details
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Sent, Paid, Overdue, Cancelled
    public string? Notes { get; set; }

    // Payment details
    public string? PaymentTerms { get; set; }
    public string? PaymentInstructions { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Items
    public List<InvoiceItem> Items { get; set; } = new();
}

public class InvoiceItem
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
}