using Microsoft.AspNetCore.Mvc;
using Moq;
using InvoiceGeneratorAPI.Controllers;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Client;
using InvoiceGeneratorAPI.DTOs.Company;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace InvoiceGeneratorAPI.Tests.Controllers;

public class ClientControllerTests
{
    private readonly Mock<IClientService> _clientServiceMock;
    private readonly Mock<ICompanyService> _companyServiceMock;
    private readonly ClientController _controller;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _companyId = Guid.NewGuid();

    public ClientControllerTests()
    {
        _clientServiceMock = new Mock<IClientService>();
        _companyServiceMock = new Mock<ICompanyService>();
        _controller = new ClientController(_clientServiceMock.Object, _companyServiceMock.Object);

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
    public async Task CreateClient_WithValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = new CreateClientRequest
        {
            Name = "Test Client",
            Email = "test@client.com"
        };

        var expectedClient = new ClientDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email
        };

        _clientServiceMock.Setup(x => x.CreateClientAsync(_companyId, request))
            .ReturnsAsync(expectedClient);

        // Act
        var result = await _controller.CreateClient(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<ClientDto>(createdResult.Value);
        Assert.Equal(expectedClient.Id, returnValue.Id);
    }

    // Add more tests for other endpoints...
}