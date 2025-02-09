namespace InvoiceGeneratorAPI.DTOs.Invoice;

public class CreateInvoiceRequest
{
    public Guid? ClientId { get; set; }

    // Client details (required if ClientId is not provided)
    public string? ClientName { get; set; }
    public string? ClientEmail { get; set; }
    public string? ClientAddress { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public string? PaymentTerms { get; set; }
    public string? PaymentInstructions { get; set; }

    public List<CreateInvoiceItemRequest> Items { get; set; } = new();
}

public class CreateInvoiceItemRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}