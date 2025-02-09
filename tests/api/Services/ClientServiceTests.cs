using Microsoft.EntityFrameworkCore;
using InvoiceGeneratorAPI.Data;
using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Client;
using Xunit;

namespace InvoiceGeneratorAPI.Tests.Services;

public class ClientServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly ClientService _service;
    private readonly Guid _companyId = Guid.NewGuid();

    public ClientServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new ClientService(_context);
    }

    [Fact]
    public async Task CreateClient_WithValidData_ReturnsClientDto()
    {
        // Arrange
        var request = new CreateClientRequest
        {
            Name = "Test Client",
            Email = "test@client.com",
            Phone = "1234567890"
        };

        // Act
        var result = await _service.CreateClientAsync(_companyId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Phone, result.Phone);
    }

    [Fact]
    public async Task GetClients_ReturnsOnlyCompanyClients()
    {
        // Arrange
        var otherCompanyId = Guid.NewGuid();
        await AddTestClient(_companyId, "Company A Client");
        await AddTestClient(otherCompanyId, "Company B Client");

        // Act
        var results = await _service.GetClientsAsync(_companyId);

        // Assert
        Assert.Single(results);
        Assert.Equal("Company A Client", results[0].Name);
    }

    [Fact]
    public async Task UpdateClient_WithValidData_UpdatesClient()
    {
        // Arrange
        var client = await AddTestClient(_companyId, "Original Name");
        var request = new UpdateClientRequest
        {
            Name = "Updated Name",
            Email = "updated@email.com"
        };

        // Act
        var result = await _service.UpdateClientAsync(_companyId, client.Id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email, result.Email);
    }

    [Fact]
    public async Task DeleteClient_RemovesClient()
    {
        // Arrange
        var client = await AddTestClient(_companyId, "Test Client");

        // Act
        var result = await _service.DeleteClientAsync(_companyId, client.Id);

        // Assert
        Assert.True(result);
        Assert.Null(await _context.Clients.FindAsync(client.Id));
    }

    private async Task<Client> AddTestClient(Guid companyId, string name)
    {
        var client = new Client
        {
            CompanyId = companyId,
            Name = name,
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }
}