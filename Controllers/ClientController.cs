using APBD_PROJECT.DataLayer.Dtos;
using APBD_PROJECT.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APBD_PROJECT.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient(
        [FromBody] ClientDto dto)
    {
        var created = await _clientService.AddClientAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ClientDto>> UpdateClient(
        long id,
        [FromBody] ClientDto dto)
    {
        var updated = await _clientService.UpdateClientAsync(id, dto);
        return Ok(updated);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ClientDto>> GetById(long id)
    {
        var client = await _clientService.GetClientAsync(id);
        return Ok(client);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _clientService.DeleteClientAsync(id);
        return NoContent();
    }
}