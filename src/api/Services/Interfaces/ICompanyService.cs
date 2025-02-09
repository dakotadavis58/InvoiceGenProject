using Microsoft.AspNetCore.Http;
using InvoiceGeneratorAPI.DTOs.Company;

namespace InvoiceGeneratorAPI.Services;

public interface ICompanyService
{
    Task<CompanyDto?> GetCompanyAsync(Guid userId);
    Task<CompanyDto> CreateCompanyAsync(Guid userId, CreateCompanyRequest request);
    Task<CompanyDto?> UpdateCompanyAsync(Guid userId, UpdateCompanyRequest request);
    Task<CompanyDto?> UpdateLogoAsync(Guid userId, IFormFile file);
    Task<CompanyDto?> RemoveLogoAsync(Guid userId);
}