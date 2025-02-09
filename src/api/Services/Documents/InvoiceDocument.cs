using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InvoiceGeneratorAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace InvoiceGeneratorAPI.Services.Documents;

public class InvoiceDocument : IDocument
{
    private readonly Invoice _invoice;
    private readonly Company _company;

    public InvoiceDocument(Invoice invoice, Company company)
    {
        _invoice = invoice;
        _company = company;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            // Company info
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(_company.Name).Bold().FontSize(20);

                // Combine address parts if they exist
                var addressParts = new List<string>();
                if (!string.IsNullOrEmpty(_company.AddressLine1))
                    addressParts.Add(_company.AddressLine1);
                if (!string.IsNullOrEmpty(_company.AddressLine2))
                    addressParts.Add(_company.AddressLine2);
                if (!string.IsNullOrEmpty(_company.City))
                    addressParts.Add(_company.City);
                if (!string.IsNullOrEmpty(_company.State))
                    addressParts.Add(_company.State);
                if (!string.IsNullOrEmpty(_company.PostalCode))
                    addressParts.Add(_company.PostalCode);
                if (!string.IsNullOrEmpty(_company.Country))
                    addressParts.Add(_company.Country);

                if (addressParts.Any())
                    column.Item().Text(string.Join(", ", addressParts));

                if (!string.IsNullOrEmpty(_company.Phone))
                    column.Item().Text(_company.Phone);
                column.Item().Text(_company.Email);
            });

            // Invoice details
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("INVOICE").Bold().FontSize(24);
                column.Item().Text($"Invoice #: {_invoice.InvoiceNumber}");
                column.Item().Text($"Issue Date: {_invoice.IssueDate:d}");
                column.Item().Text($"Due Date: {_invoice.DueDate:d}");
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // Bill to section
            column.Item().Text("Bill To:").Bold();
            column.Item().Column(c =>
            {
                c.Item().Text(_invoice.ClientName);
                c.Item().Text(_invoice.ClientEmail);
                if (!string.IsNullOrEmpty(_invoice.ClientAddress))
                    c.Item().Text(_invoice.ClientAddress);
            });

            // Items table
            column.Item().PaddingVertical(20).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                // Table header
                table.Header(header =>
                {
                    header.Cell().Text("Description").Bold();
                    header.Cell().Text("Quantity").Bold();
                    header.Cell().Text("Unit Price").Bold();
                    header.Cell().AlignRight().Text("Amount").Bold();
                });

                // Items
                foreach (var item in _invoice.Items)
                {
                    table.Cell().Text(item.Description);
                    table.Cell().Text($"{item.Quantity:N2}");
                    table.Cell().Text($"${item.UnitPrice:N2}");
                    table.Cell().AlignRight().Text($"${item.Amount:N2}");
                }
            });

            // Totals
            column.Item().AlignRight().Column(c =>
            {
                c.Item().Text($"Subtotal: ${_invoice.SubTotal:N2}");
                c.Item().Text($"Tax ({_invoice.TaxRate}%): ${_invoice.TaxAmount:N2}");
                c.Item().Text($"Total: ${_invoice.TotalAmount:N2}").Bold();
            });

            // Notes
            if (!string.IsNullOrEmpty(_invoice.Notes))
            {
                column.Item().PaddingTop(20).Column(c =>
                {
                    c.Item().Text("Notes:").Bold();
                    c.Item().Text(_invoice.Notes);
                });
            }

            // Payment details
            if (!string.IsNullOrEmpty(_invoice.PaymentInstructions))
            {
                column.Item().PaddingTop(20).Column(c =>
                {
                    c.Item().Text("Payment Instructions:").Bold();
                    c.Item().Text(_invoice.PaymentInstructions);
                });
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.PaddingTop(20).Column(column =>
        {
            if (!string.IsNullOrEmpty(_invoice.PaymentTerms))
            {
                column.Item().Text("Payment Terms:").Bold();
                column.Item().Text(_invoice.PaymentTerms);
            }
        });
    }
}