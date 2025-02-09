using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using InvoiceGeneratorAPI.Controllers;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.DTOs.Company;
using System.Security.Claims;
using FluentAssertions;
using Xunit;

namespace InvoiceGeneratorAPI.Tests.Controllers;

public class CompanyControllerTests
{
    private readonly Mock<ICompanyService> _companyServiceMock;
    private readonly CompanyController _controller;
    private readonly Guid _userId;

    public CompanyControllerTests()
    {
        _companyServiceMock = new Mock<ICompanyService>();
        _controller = new CompanyController(_companyServiceMock.Object);
        _userId = Guid.NewGuid();

        // Setup controller context with user
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, _userId.ToString())
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetCompany_WhenCompanyExists_ReturnsCompany()
    {
        // Arrange
        var company = new CompanyDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Email = "test@company.com"
        };

        _companyServiceMock.Setup(x => x.GetCompanyAsync(_userId))
            .ReturnsAsync(company);

        // Act
        var result = await _controller.GetCompany();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedCompany = okResult!.Value as CompanyDto;
        returnedCompany.Should().NotBeNull();
        returnedCompany!.Name.Should().Be("Test Company");
    }

    [Fact]
    public async Task GetCompany_WhenNoCompany_ReturnsNotFound()
    {
        // Arrange
        _companyServiceMock.Setup(x => x.GetCompanyAsync(_userId))
            .ReturnsAsync((CompanyDto?)null);

        // Act
        var result = await _controller.GetCompany();

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateCompany_WithValidData_ReturnsCreatedCompany()
    {
        // Arrange
        var request = new CreateCompanyRequest
        {
            Name = "New Company",
            Email = "new@company.com"
        };

        var createdCompany = new CompanyDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email
        };

        _companyServiceMock.Setup(x => x.CreateCompanyAsync(_userId, request))
            .ReturnsAsync(createdCompany);

        // Act
        var result = await _controller.CreateCompany(request);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        var returnedCompany = createdResult!.Value as CompanyDto;
        returnedCompany.Should().NotBeNull();
        returnedCompany!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateCompany_WithValidData_ReturnsUpdatedCompany()
    {
        // Arrange
        var request = new UpdateCompanyRequest
        {
            Name = "Updated Company",
            Email = "updated@company.com"
        };

        var updatedCompany = new CompanyDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email
        };

        _companyServiceMock.Setup(x => x.UpdateCompanyAsync(_userId, request))
            .ReturnsAsync(updatedCompany);

        // Act
        var result = await _controller.UpdateCompany(request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedCompany = okResult!.Value as CompanyDto;
        returnedCompany.Should().NotBeNull();
        returnedCompany!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateCompany_WhenNoCompany_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateCompanyRequest
        {
            Name = "Updated Company"
        };

        _companyServiceMock.Setup(x => x.UpdateCompanyAsync(_userId, request))
            .ReturnsAsync((CompanyDto?)null);

        // Act
        var result = await _controller.UpdateCompany(request);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}