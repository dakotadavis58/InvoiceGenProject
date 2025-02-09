using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Invoice;
using InvoiceGeneratorAPI.Extensions;
using InvoiceGeneratorAPI.Exceptions;

namespace InvoiceGeneratorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly ICompanyService _companyService;

    public InvoiceController(IInvoiceService invoiceService, ICompanyService companyService)
    {
        _invoiceService = invoiceService;
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<InvoiceDto>>> GetInvoices()
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var invoices = await _invoiceService.GetInvoicesAsync(company.Id);
        return Ok(invoices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(Guid id)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var invoice = await _invoiceService.GetInvoiceAsync(company.Id, id);

        if (invoice == null)
            return NotFound();

        return Ok(invoice);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> CreateInvoice(CreateInvoiceRequest request)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var invoice = await _invoiceService.CreateInvoiceAsync(company.Id, request);
        return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<InvoiceDto>> UpdateInvoice(Guid id, UpdateInvoiceRequest request)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var invoice = await _invoiceService.UpdateInvoiceAsync(company.Id, id, request);

        if (invoice == null)
            return NotFound();

        return Ok(invoice);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInvoice(Guid id)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var success = await _invoiceService.DeleteInvoiceAsync(company.Id, id);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/send")]
    public async Task<ActionResult> SendInvoice(Guid id)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        try
        {
            await _invoiceService.SendInvoiceAsync(company.Id, id);
            return Ok();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/download")]
    public async Task<ActionResult> DownloadInvoice(Guid id)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        try
        {
            var pdfBytes = await _invoiceService.GeneratePdfAsync(company.Id, id);
            return File(pdfBytes, "application/pdf", $"invoice-{id}.pdf");
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}