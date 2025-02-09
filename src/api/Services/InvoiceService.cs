using InvoiceGeneratorAPI.Data;
using InvoiceGeneratorAPI.Models;
using Microsoft.EntityFrameworkCore;
using InvoiceGeneratorAPI.DTOs.Invoice;
using InvoiceGeneratorAPI.Exceptions;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using InvoiceGeneratorAPI.Services.Documents;
using QuestPDF.Fluent;

namespace InvoiceGeneratorAPI.Services;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;

    public InvoiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InvoiceDto>> GetInvoicesAsync(Guid companyId)
    {
        var invoices = await _context.Invoices
            .Include(i => i.Items)
            .Where(i => i.CompanyId == companyId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return invoices.Select(MapToDto).ToList();
    }

    public async Task<InvoiceDto?> GetInvoiceAsync(Guid companyId, Guid id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.CompanyId == companyId && i.Id == id);

        return invoice != null ? MapToDto(invoice) : null;
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(Guid companyId, CreateInvoiceRequest request)
    {
        // If ClientId is provided, verify it exists and belongs to the company
        if (request.ClientId.HasValue)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Id == request.ClientId);

            if (client == null)
                throw new Exceptions.NotFoundException($"Client with ID {request.ClientId} not found");
        }
        else if (string.IsNullOrEmpty(request.ClientName) || string.IsNullOrEmpty(request.ClientEmail))
        {
            throw new ValidationException("Client name and email are required when no client ID is provided");
        }

        var invoice = new Invoice
        {
            CompanyId = companyId,
            ClientId = request.ClientId,
            ClientName = request.ClientId.HasValue ? string.Empty : request.ClientName!,
            ClientEmail = request.ClientId.HasValue ? string.Empty : request.ClientEmail!,
            ClientAddress = request.ClientAddress,
            InvoiceNumber = await GenerateInvoiceNumberAsync(companyId),
            IssueDate = request.IssueDate,
            DueDate = request.DueDate,
            TaxRate = request.TaxRate,
            Notes = request.Notes,
            PaymentTerms = request.PaymentTerms,
            PaymentInstructions = request.PaymentInstructions,
            Status = "Draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add items and calculate totals
        foreach (var item in request.Items)
        {
            var invoiceItem = new InvoiceItem
            {
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Amount = item.Quantity * item.UnitPrice
            };
            invoice.Items.Add(invoiceItem);
        }

        // Calculate totals
        invoice.SubTotal = invoice.Items.Sum(i => i.Amount);
        invoice.TaxAmount = invoice.SubTotal * (invoice.TaxRate / 100);
        invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return MapToDto(invoice);
    }

    private async Task<string> GenerateInvoiceNumberAsync(Guid companyId)
    {
        var company = await _context.Companies.FindAsync(companyId);
        var prefix = company?.InvoicePrefix ?? "INV";

        var lastInvoice = await _context.Invoices
            .Where(i => i.CompanyId == companyId)
            .OrderByDescending(i => i.CreatedAt)
            .FirstOrDefaultAsync();

        int number = 1;
        if (lastInvoice != null)
        {
            var lastNumber = lastInvoice.InvoiceNumber.Replace(prefix, "");
            if (int.TryParse(lastNumber, out int lastSeq))
            {
                number = lastSeq + 1;
            }
        }

        return $"{prefix}{number:D5}";
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CompanyId = invoice.CompanyId,
            ClientId = invoice.ClientId,
            ClientName = invoice.ClientName,
            ClientEmail = invoice.ClientEmail,
            ClientAddress = invoice.ClientAddress ?? string.Empty,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            SubTotal = invoice.SubTotal,
            TaxRate = invoice.TaxRate,
            TaxAmount = invoice.TaxAmount,
            TotalAmount = invoice.TotalAmount,
            Status = invoice.Status,
            Notes = invoice.Notes ?? string.Empty,
            PaymentTerms = invoice.PaymentTerms ?? string.Empty,
            PaymentInstructions = invoice.PaymentInstructions ?? string.Empty,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt,
            Items = invoice.Items.Select(i => new InvoiceItemDto
            {
                Id = i.Id,
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Amount = i.Amount
            }).ToList()
        };
    }

    public async Task<InvoiceDto?> UpdateInvoiceAsync(Guid companyId, Guid id, UpdateInvoiceRequest request)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.CompanyId == companyId && i.Id == id);

        if (invoice == null)
            return null;

        // Update client information
        if (request.ClientId.HasValue)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Id == request.ClientId);

            if (client == null)
                throw new Exceptions.NotFoundException($"Client with ID {request.ClientId} not found");

            invoice.ClientId = request.ClientId;
            invoice.ClientName = string.Empty;
            invoice.ClientEmail = string.Empty;
        }
        else if (request.ClientName != null || request.ClientEmail != null)
        {
            invoice.ClientId = null;
            invoice.ClientName = request.ClientName ?? invoice.ClientName;
            invoice.ClientEmail = request.ClientEmail ?? invoice.ClientEmail;
        }

        // Update other fields if provided
        if (request.ClientAddress != null)
            invoice.ClientAddress = request.ClientAddress;
        if (request.IssueDate.HasValue)
            invoice.IssueDate = request.IssueDate.Value;
        if (request.DueDate.HasValue)
            invoice.DueDate = request.DueDate.Value;
        if (request.TaxRate.HasValue)
            invoice.TaxRate = request.TaxRate.Value;
        if (request.Status != null)
            invoice.Status = request.Status;
        if (request.Notes != null)
            invoice.Notes = request.Notes;
        if (request.PaymentTerms != null)
            invoice.PaymentTerms = request.PaymentTerms;
        if (request.PaymentInstructions != null)
            invoice.PaymentInstructions = request.PaymentInstructions;

        // Update items if provided
        if (request.Items != null)
        {
            // Remove existing items
            _context.InvoiceItems.RemoveRange(invoice.Items);
            invoice.Items.Clear();

            // Add new items
            foreach (var item in request.Items)
            {
                var invoiceItem = new InvoiceItem
                {
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Amount = item.Quantity * item.UnitPrice
                };
                invoice.Items.Add(invoiceItem);
            }

            // Recalculate totals
            invoice.SubTotal = invoice.Items.Sum(i => i.Amount);
            invoice.TaxAmount = invoice.SubTotal * (invoice.TaxRate / 100);
            invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount;
        }

        invoice.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(invoice);
    }

    public async Task<bool> DeleteInvoiceAsync(Guid companyId, Guid id)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.CompanyId == companyId && i.Id == id);

        if (invoice == null)
            return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<byte[]> GeneratePdfAsync(Guid companyId, Guid id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Company)
            .FirstOrDefaultAsync(i => i.CompanyId == companyId && i.Id == id);

        if (invoice == null)
            throw new Exceptions.NotFoundException("Invoice not found");

        var document = new InvoiceDocument(invoice, invoice.Company);
        return document.GeneratePdf();
    }

    public async Task SendInvoiceAsync(Guid companyId, Guid id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Company)
            .FirstOrDefaultAsync(i => i.CompanyId == companyId && i.Id == id);

        if (invoice == null)
            throw new Exceptions.NotFoundException("Invoice not found");

        // Generate PDF
        var pdfBytes = await GeneratePdfAsync(companyId, id);

        // Create SES client
        var sesClient = new AmazonSimpleEmailServiceV2Client();
        var fromAddress = invoice.Company.Email;
        if (string.IsNullOrEmpty(fromAddress))
            throw new Exceptions.NotFoundException("Company email not found");

        // Create email request
        var request = new SendEmailRequest
        {
            FromEmailAddress = invoice.Company.Email,
            Destination = new Destination
            {
                ToAddresses = new List<string> { invoice.ClientEmail }
            },
            Content = new EmailContent
            {
                Raw = new RawMessage
                {
                    Data = new MemoryStream(CreateRawEmailWithAttachment(
                        fromAddress: invoice.Company.Email,
                        toAddress: invoice.ClientEmail,
                        subject: $"Invoice {invoice.InvoiceNumber} from {invoice.Company.Name}",
                        textBody: $"Please find attached invoice {invoice.InvoiceNumber}.\n\n" +
                                 $"Amount Due: ${invoice.TotalAmount:N2}\n" +
                                 $"Due Date: {invoice.DueDate:d}\n\n" +
                                 "Thank you for your business.",
                        pdfAttachment: pdfBytes,
                        filename: $"invoice-{invoice.InvoiceNumber}.pdf"
                    ))
                }
            }
        };

        // Send email
        await sesClient.SendEmailAsync(request);

        // Update invoice status
        invoice.Status = "Sent";
        invoice.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private byte[] CreateRawEmailWithAttachment(string fromAddress, string toAddress, string subject,
        string textBody, byte[] pdfAttachment, string filename)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, leaveOpen: true);

        // Write email headers
        writer.WriteLine($"From: {fromAddress}");
        writer.WriteLine($"To: {toAddress}");
        writer.WriteLine($"Subject: {subject}");
        writer.WriteLine("MIME-Version: 1.0");

        // Generate boundary
        string boundary = $"------------{DateTime.Now.Ticks:x}";
        writer.WriteLine($"Content-Type: multipart/mixed; boundary=\"{boundary}\"");
        writer.WriteLine();

        // Write text part
        writer.WriteLine($"--{boundary}");
        writer.WriteLine("Content-Type: text/plain; charset=utf-8");
        writer.WriteLine("Content-Transfer-Encoding: quoted-printable");
        writer.WriteLine();
        writer.WriteLine(textBody);
        writer.WriteLine();

        // Write PDF attachment
        writer.WriteLine($"--{boundary}");
        writer.WriteLine($"Content-Type: application/pdf; name=\"{filename}\"");
        writer.WriteLine("Content-Transfer-Encoding: base64");
        writer.WriteLine($"Content-Disposition: attachment; filename=\"{filename}\"");
        writer.WriteLine();
        writer.WriteLine(Convert.ToBase64String(pdfAttachment));
        writer.WriteLine();

        // Write final boundary
        writer.WriteLine($"--{boundary}--");
        writer.Flush();

        return stream.ToArray();
    }
}