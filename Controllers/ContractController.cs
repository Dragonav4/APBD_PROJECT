using APBD_PROJECT.DataLayer.Dtos;
using APBD_PROJECT.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace APBD_PROJECT.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpPost]
    public async Task<ActionResult<ContractPayments.ContractResponseDto>> CreateContract(
        [FromBody] ContractPayments.ContractCreateDto dto)
    {
        var result = await _contractService.CreateContractAsync(dto);
        return CreatedAtAction(nameof(GetContractById), new { id = result.Id }, result);
    }

    [HttpGet("{id:long}", Name = "GetContractById")]
    public async Task<ActionResult<ContractPayments.ContractResponseDto>> GetContractById(long id)
    {
        var dto = (await _contractService.GetContractsForClientAsync(id))
            .FirstOrDefault(c => c.Id == id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("client/{clientId:long}")]
    public async Task<ActionResult<IEnumerable<ContractPayments.ContractResponseDto>>> GetContractsForClient(long clientId)
    {
        var dtos = await _contractService.GetContractsForClientAsync(clientId);
        return Ok(dtos);
    }

    [HttpPost("{contractId:long}/payments")]
    public async Task<IActionResult> AddPayment(
        long contractId,
        [FromBody] [Required] decimal amount)
    {
        await _contractService.AddPaymentAsync(contractId, amount);
        return NoContent();
    }

    [HttpGet("{contractId:long}/isRevenueRecognized")]
    public async Task<ActionResult<bool>> IsRevenueRecognized(long contractId)
    {
        var recognized = await _contractService.IsRevenueRecognized(contractId);
        return Ok(recognized);
    }
}