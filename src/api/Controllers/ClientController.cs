using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Client;
using InvoiceGeneratorAPI.Extensions;

namespace InvoiceGeneratorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ICompanyService _companyService;

    public ClientController(IClientService clientService, ICompanyService companyService)
    {
        _clientService = clientService;
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> GetClients()
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var clients = await _clientService.GetClientsAsync(company.Id);
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetClient(Guid id)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var client = await _clientService.GetClientAsync(company.Id, id);

        if (client == null)
            return NotFound();

        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient(CreateClientRequest request)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var client = await _clientService.CreateClientAsync(company.Id, request);
        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDto>> UpdateClient(Guid id, UpdateClientRequest request)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var client = await _clientService.UpdateClientAsync(company.Id, id, request);

        if (client == null)
            return NotFound();

        return Ok(client);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteClient(Guid id)
    {
        var userId = User.GetUserId();
        var company = await _companyService.GetCompanyAsync(userId);

        if (company == null)
            return NotFound("Company not found");

        var success = await _clientService.DeleteClientAsync(company.Id, id);

        if (!success)
            return NotFound();

        return NoContent();
    }
}