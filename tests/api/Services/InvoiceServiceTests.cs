using Microsoft.EntityFrameworkCore;
using InvoiceGeneratorAPI.Data;
using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Invoice;
using Xunit;

namespace InvoiceGeneratorAPI.Tests.Services;

public class InvoiceServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly InvoiceService _service;
    private readonly Guid _companyId = Guid.NewGuid();

    public InvoiceServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new InvoiceService(_context);
    }

    [Fact]
    public async Task CreateInvoice_WithValidData_CreatesInvoice()
    {
        // Arrange
        var request = new CreateInvoiceRequest
        {
            ClientName = "Test Client",
            ClientEmail = "test@client.com",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            TaxRate = 10,
            Items = new List<CreateInvoiceItemRequest>
            {
                new()
                {
                    Description = "Test Item",
                    Quantity = 1,
                    UnitPrice = 100
                }
            }
        };

        // Act
        var result = await _service.CreateInvoiceAsync(_companyId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.ClientName, result.ClientName);
        Assert.Equal(request.ClientEmail, result.ClientEmail);
        Assert.Single(result.Items);
        Assert.Equal(100, result.SubTotal);
        Assert.Equal(10, result.TaxAmount);
        Assert.Equal(110, result.TotalAmount);
    }

    [Fact]
    public async Task UpdateInvoice_WithValidData_UpdatesInvoice()
    {
        // Arrange
        var invoice = await CreateTestInvoice();
        var request = new UpdateInvoiceRequest
        {
            ClientName = "Updated Client",
            Status = "Sent"
        };

        // Act
        var result = await _service.UpdateInvoiceAsync(_companyId, invoice.Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.ClientName, result.ClientName);
        Assert.Equal(request.Status, result.Status);
    }

    [Fact]
    public async Task DeleteInvoice_RemovesInvoice()
    {
        // Arrange
        var invoice = await CreateTestInvoice();

        // Act
        var result = await _service.DeleteInvoiceAsync(_companyId, invoice.Id);

        // Assert
        Assert.True(result);
        Assert.Null(await _context.Invoices.FindAsync(invoice.Id));
    }

    private async Task<Invoice> CreateTestInvoice()
    {
        var invoice = new Invoice
        {
            CompanyId = _companyId,
            ClientName = "Test Client",
            ClientEmail = "test@client.com",
            InvoiceNumber = "INV00001",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = "Draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = new List<InvoiceItem>
            {
                new()
                {
                    Description = "Test Item",
                    Quantity = 1,
                    UnitPrice = 100,
                    Amount = 100
                }
            }
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }
}