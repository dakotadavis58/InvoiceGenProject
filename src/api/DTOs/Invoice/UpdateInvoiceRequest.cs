namespace InvoiceGeneratorAPI.DTOs.Invoice;

public class UpdateInvoiceRequest
{
    public Guid? ClientId { get; set; }
    public string? ClientName { get; set; }
    public string? ClientEmail { get; set; }
    public string? ClientAddress { get; set; }

    public DateTime? IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? TaxRate { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public string? PaymentTerms { get; set; }
    public string? PaymentInstructions { get; set; }

    public List<CreateInvoiceItemRequest>? Items { get; set; }
}