using APBD_PROJECT.DataLayer.Dtos;
using APBD_PROJECT.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APBD_PROJECT.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public SubscriptionsController(ISubscriptionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SubscriptionsDto.SubscriptionCreateDto dto)
    {
        var subscription = await _service.CreateSubscriptionAsync(dto);
        return Ok(subscription);
    }

    [HttpPost("{id:long}/renew")]
    public async Task<IActionResult> Renew(long id, [FromQuery] decimal amount)
    {
        await _service.AddRenewalPaymentAsync(id, amount);
        return NoContent();
    }

    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> Cancel(long id)
    {
        await _service.CancelSubscriptionAsync(id);
        return NoContent();
    }
    
    [HttpGet("client/{clientId:long}")]
    public async Task<IActionResult> GetForClient(long clientId)
    {
        var subs = await _service.GetSubscriptionByIdAsync(clientId);
        return Ok(subs);
    }

    [HttpGet("{id:long}/next-renewal")]
    public async Task<IActionResult> GetNextRenewal(long id)
    {
        var next = await _service.GetNextRenewalPeriodAsync(id);
        return Ok(next);
    }

    [HttpGet("revenue/current")]
    public async Task<IActionResult> GetCurrentRevenue()
    {
        var revenue = await _service.GetCurrentSubscriptionRevenueAsync();
        return Ok(revenue);
    }

    [HttpGet("revenue/predicted")]
    public async Task<IActionResult> GetPredictedRevenue()
    {
        var revenue = await _service.GetPredictedSubscriptionRevenueAsync();
        return Ok(revenue);
    }
}