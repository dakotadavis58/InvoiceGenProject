using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using InvoiceGeneratorAPI.Controllers;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Invoice;
using InvoiceGeneratorAPI.DTOs.Company;
using Xunit;
using System.Security.Claims;

namespace InvoiceGeneratorAPI.Tests.Controllers;

public class InvoiceControllerTests
{
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly Mock<ICompanyService> _companyServiceMock;
    private readonly InvoiceController _controller;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _companyId = Guid.NewGuid();

    public InvoiceControllerTests()
    {
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _companyServiceMock = new Mock<ICompanyService>();
        _controller = new InvoiceController(_invoiceServiceMock.Object, _companyServiceMock.Object);

        // Setup user claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, _userId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Setup company service mock
        _companyServiceMock.Setup(x => x.GetCompanyAsync(_userId))
            .ReturnsAsync(new CompanyDto { Id = _companyId });
    }

    [Fact]
    public async Task GetInvoices_ReturnsInvoiceList()
    {
        // Arrange
        var expectedInvoices = new List<InvoiceDto>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV00001" },
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV00002" }
        };

        _invoiceServiceMock.Setup(x => x.GetInvoicesAsync(_companyId))
            .ReturnsAsync(expectedInvoices);

        // Act
        var result = await _controller.GetInvoices();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<InvoiceDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetInvoice_WithValidId_ReturnsInvoice()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var expectedInvoice = new InvoiceDto
        {
            Id = invoiceId,
            InvoiceNumber = "INV00001"
        };

        _invoiceServiceMock.Setup(x => x.GetInvoiceAsync(_companyId, invoiceId))
            .ReturnsAsync(expectedInvoice);

        // Act
        var result = await _controller.GetInvoice(invoiceId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<InvoiceDto>(okResult.Value);
        Assert.Equal(invoiceId, returnValue.Id);
    }

    [Fact]
    public async Task CreateInvoice_WithValidRequest_ReturnsCreatedInvoice()
    {
        // Arrange
        var request = new CreateInvoiceRequest
        {
            ClientName = "Test Client",
            ClientEmail = "test@client.com",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
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

        var createdInvoice = new InvoiceDto
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = "INV00001",
            ClientName = request.ClientName,
            ClientEmail = request.ClientEmail
        };

        _invoiceServiceMock.Setup(x => x.CreateInvoiceAsync(_companyId, request))
            .ReturnsAsync(createdInvoice);

        // Act
        var result = await _controller.CreateInvoice(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<InvoiceDto>(createdResult.Value);
        Assert.Equal(request.ClientName, returnValue.ClientName);
    }

    [Fact]
    public async Task UpdateInvoice_WithValidRequest_ReturnsUpdatedInvoice()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var request = new UpdateInvoiceRequest
        {
            ClientName = "Updated Client",
            Status = "Sent"
        };

        var updatedInvoice = new InvoiceDto
        {
            Id = invoiceId,
            ClientName = request.ClientName,
            Status = request.Status
        };

        _invoiceServiceMock.Setup(x => x.UpdateInvoiceAsync(_companyId, invoiceId, request))
            .ReturnsAsync(updatedInvoice);

        // Act
        var result = await _controller.UpdateInvoice(invoiceId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<InvoiceDto>(okResult.Value);
        Assert.Equal(request.ClientName, returnValue.ClientName);
        Assert.Equal(request.Status, returnValue.Status);
    }

    [Fact]
    public async Task DeleteInvoice_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        _invoiceServiceMock.Setup(x => x.DeleteInvoiceAsync(_companyId, invoiceId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteInvoice(invoiceId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetInvoice_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        _invoiceServiceMock.Setup(x => x.GetInvoiceAsync(_companyId, invoiceId))
            .ReturnsAsync((InvoiceDto?)null);

        // Act
        var result = await _controller.GetInvoice(invoiceId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetInvoices_WithNoCompany_ReturnsNotFound()
    {
        // Arrange
        _companyServiceMock.Setup(x => x.GetCompanyAsync(_userId))
            .ReturnsAsync((CompanyDto?)null);

        // Act
        var result = await _controller.GetInvoices();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Company not found", notFoundResult.Value);
    }
}