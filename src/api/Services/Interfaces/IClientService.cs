using InvoiceGeneratorAPI.DTOs.Client;

namespace InvoiceGeneratorAPI.Services;

public interface IClientService
{
    Task<List<ClientDto>> GetClientsAsync(Guid companyId);
    Task<ClientDto?> GetClientAsync(Guid companyId, Guid id);
    Task<ClientDto> CreateClientAsync(Guid companyId, CreateClientRequest request);
    Task<ClientDto?> UpdateClientAsync(Guid companyId, Guid id, UpdateClientRequest request);
    Task<bool> DeleteClientAsync(Guid companyId, Guid id);
}