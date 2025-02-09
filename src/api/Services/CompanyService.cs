using Microsoft.EntityFrameworkCore;
using InvoiceGeneratorAPI.Data;
using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.DTOs.Company;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using InvoiceGeneratorAPI.Settings;

namespace InvoiceGeneratorAPI.Services;

public class CompanyService(
    ApplicationDbContext context,
    IWebHostEnvironment environment,
    IOptions<AwsSettings> awsSettings) : ICompanyService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly AmazonS3Client _s3Client = new(
            awsSettings.Value.AccessKey,
            awsSettings.Value.SecretKey,
            Amazon.RegionEndpoint.GetBySystemName(awsSettings.Value.Region)
        );
    private readonly string _bucketName = awsSettings.Value.BucketName;

    public async Task<CompanyDto?> GetCompanyAsync(Guid userId)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (company == null)
            return null;

        return MapToDto(company);
    }

    public async Task<CompanyDto> CreateCompanyAsync(Guid userId, CreateCompanyRequest request)
    {
        var company = new Company
        {
            UserId = userId,
            Name = request.Name,
            Email = request.Email,
            Website = request.Website,
            Phone = request.Phone,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            TaxNumber = request.TaxNumber,
            RegistrationNumber = request.RegistrationNumber,
            InvoicePrefix = request.InvoicePrefix,
            InvoiceNotes = request.InvoiceNotes,
            PaymentInstructions = request.PaymentInstructions,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return MapToDto(company);
    }

    public async Task<CompanyDto?> UpdateCompanyAsync(Guid userId, UpdateCompanyRequest request)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (company == null)
            return null;

        // Update only the properties that are provided
        if (request.Name != null)
            company.Name = request.Name;
        if (request.Email != null)
            company.Email = request.Email;
        if (request.Website != null)
            company.Website = request.Website;
        if (request.Phone != null)
            company.Phone = request.Phone;
        if (request.AddressLine1 != null)
            company.AddressLine1 = request.AddressLine1;
        if (request.AddressLine2 != null)
            company.AddressLine2 = request.AddressLine2;
        if (request.City != null)
            company.City = request.City;
        if (request.State != null)
            company.State = request.State;
        if (request.PostalCode != null)
            company.PostalCode = request.PostalCode;
        if (request.Country != null)
            company.Country = request.Country;
        if (request.TaxNumber != null)
            company.TaxNumber = request.TaxNumber;
        if (request.RegistrationNumber != null)
            company.RegistrationNumber = request.RegistrationNumber;
        if (request.InvoicePrefix != null)
            company.InvoicePrefix = request.InvoicePrefix;
        if (request.InvoiceNotes != null)
            company.InvoiceNotes = request.InvoiceNotes;
        if (request.PaymentInstructions != null)
            company.PaymentInstructions = request.PaymentInstructions;

        company.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(company);
    }

    public async Task<CompanyDto?> UpdateLogoAsync(Guid userId, IFormFile file)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (company == null)
            return null;

        // Delete old logo if exists
        if (!string.IsNullOrEmpty(company.LogoUrl))
        {
            var oldKey = Path.GetFileName(company.LogoUrl);
            await _s3Client.DeleteObjectAsync(_bucketName, $"logos/{oldKey}");
        }

        // Upload new logo
        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var key = $"logos/{fileName}";

        using (var newMemoryStream = new MemoryStream())
        {
            await file.CopyToAsync(newMemoryStream);
            newMemoryStream.Position = 0;

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = key,
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);
        }

        // Update company with S3 URL
        company.LogoUrl = $"https://{_bucketName}.s3.amazonaws.com/{key}";
        company.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(company);
    }

    public async Task<CompanyDto?> RemoveLogoAsync(Guid userId)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (company == null)
            return null;

        if (!string.IsNullOrEmpty(company.LogoUrl))
        {
            var key = $"logos/{Path.GetFileName(company.LogoUrl)}";
            await _s3Client.DeleteObjectAsync(_bucketName, key);

            company.LogoUrl = null;
            company.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return MapToDto(company);
    }

    private static CompanyDto MapToDto(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            LogoUrl = company.LogoUrl,
            Website = company.Website ?? string.Empty,
            Phone = company.Phone ?? string.Empty,
            Email = company.Email ?? string.Empty,
            AddressLine1 = company.AddressLine1 ?? string.Empty,
            AddressLine2 = company.AddressLine2 ?? string.Empty,
            City = company.City ?? string.Empty,
            State = company.State ?? string.Empty,
            PostalCode = company.PostalCode ?? string.Empty,
            Country = company.Country ?? string.Empty,
            TaxNumber = company.TaxNumber ?? string.Empty,
            RegistrationNumber = company.RegistrationNumber ?? string.Empty,
            InvoicePrefix = company.InvoicePrefix ?? string.Empty,
            InvoiceNotes = company.InvoiceNotes ?? string.Empty,
            PaymentInstructions = company.PaymentInstructions ?? string.Empty
        };
    }
}