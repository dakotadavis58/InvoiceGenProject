using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.DTOs.Invoice;

namespace InvoiceGeneratorAPI.Services;

public interface IInvoiceService
{
    Task<List<InvoiceDto>> GetInvoicesAsync(Guid companyId);
    Task<InvoiceDto?> GetInvoiceAsync(Guid companyId, Guid id);
    Task<InvoiceDto> CreateInvoiceAsync(Guid companyId, CreateInvoiceRequest request);
    Task<InvoiceDto?> UpdateInvoiceAsync(Guid companyId, Guid id, UpdateInvoiceRequest request);
    Task<bool> DeleteInvoiceAsync(Guid companyId, Guid id);
    Task<byte[]> GeneratePdfAsync(Guid companyId, Guid id);
    Task SendInvoiceAsync(Guid companyId, Guid id);
}