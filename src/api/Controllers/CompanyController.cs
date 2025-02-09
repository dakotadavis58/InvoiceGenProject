using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Company;
using InvoiceGeneratorAPI.Extensions;

namespace InvoiceGeneratorAPI.Controllers;

/// <summary>
/// Manages company information and settings
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Gets the company information for the authenticated user
    /// </summary>
    /// <returns>The company information</returns>
    /// <response code="200">Returns the company information</response>
    /// <response code="404">If the company doesn't exist</response>
    [HttpGet]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> GetCompany()
    {
        var userId = User.GetUserId();
        Console.WriteLine("userId fetching company: " + userId);
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    /// <summary>
    /// Creates a new company for the authenticated user
    /// </summary>
    /// <param name="request">The company information</param>
    /// <returns>The created company</returns>
    /// <response code="201">Returns the newly created company</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyRequest request)
    {
        var userId = User.GetUserId();
        var company = await _companyService.CreateCompanyAsync(userId, request);

        return CreatedAtAction(nameof(GetCompany), company);
    }

    [HttpPut]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(UpdateCompanyRequest request)
    {
        var userId = User.GetUserId();
        var company = await _companyService.UpdateCompanyAsync(userId, request);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    /// <summary>
    /// Updates the company logo
    /// </summary>
    /// <param name="file">The logo image file (jpg, png, gif)</param>
    /// <returns>The updated company information</returns>
    /// <response code="200">Returns the updated company</response>
    /// <response code="400">If the file is invalid</response>
    /// <response code="404">If the company doesn't exist</response>
    [HttpPost("logo")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> UploadLogo(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.ContentType.StartsWith("image/"))
            return BadRequest("File must be an image");

        var userId = User.GetUserId();
        var company = await _companyService.UpdateLogoAsync(userId, file);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [HttpDelete("logo")]
    public async Task<ActionResult<CompanyDto>> RemoveLogo()
    {
        var userId = User.GetUserId();
        var company = await _companyService.RemoveLogoAsync(userId);

        if (company == null)
            return NotFound();

        return Ok(company);
    }
}