using Microsoft.EntityFrameworkCore;
using InvoiceGeneratorAPI.Data;
using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.DTOs.Client;
using InvoiceGeneratorAPI.Exceptions;

namespace InvoiceGeneratorAPI.Services;

public class ClientService : IClientService
{
    private readonly ApplicationDbContext _context;

    public ClientService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClientDto>> GetClientsAsync(Guid companyId)
    {
        var clients = await _context.Clients
            .Where(c => c.CompanyId == companyId)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return clients.Select(MapToDto).ToList();
    }

    public async Task<ClientDto?> GetClientAsync(Guid companyId, Guid id)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Id == id);

        return client != null ? MapToDto(client) : null;
    }

    public async Task<ClientDto> CreateClientAsync(Guid companyId, CreateClientRequest request)
    {
        var client = new Client
        {
            CompanyId = companyId,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            TaxNumber = request.TaxNumber,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return MapToDto(client);
    }

    public async Task<ClientDto?> UpdateClientAsync(Guid companyId, Guid id, UpdateClientRequest request)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Id == id);

        if (client == null)
            return null;

        if (request.Name != null)
            client.Name = request.Name;
        if (request.Email != null)
            client.Email = request.Email;
        if (request.Phone != null)
            client.Phone = request.Phone;
        if (request.AddressLine1 != null)
            client.AddressLine1 = request.AddressLine1;
        if (request.AddressLine2 != null)
            client.AddressLine2 = request.AddressLine2;
        if (request.City != null)
            client.City = request.City;
        if (request.State != null)
            client.State = request.State;
        if (request.PostalCode != null)
            client.PostalCode = request.PostalCode;
        if (request.Country != null)
            client.Country = request.Country;
        if (request.TaxNumber != null)
            client.TaxNumber = request.TaxNumber;
        if (request.Notes != null)
            client.Notes = request.Notes;

        client.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(client);
    }

    public async Task<bool> DeleteClientAsync(Guid companyId, Guid id)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Id == id);

        if (client == null)
            return false;

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return true;
    }

    private static ClientDto MapToDto(Client client)
    {
        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone ?? string.Empty,
            AddressLine1 = client.AddressLine1 ?? string.Empty,
            AddressLine2 = client.AddressLine2 ?? string.Empty,
            City = client.City ?? string.Empty,
            State = client.State ?? string.Empty,
            PostalCode = client.PostalCode ?? string.Empty,
            Country = client.Country ?? string.Empty,
            TaxNumber = client.TaxNumber ?? string.Empty,
            Notes = client.Notes ?? string.Empty,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };
    }
}