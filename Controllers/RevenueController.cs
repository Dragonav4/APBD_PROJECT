using APBD_PROJECT.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APBD_PROJECT.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    [HttpGet("actual")]
    public async Task<IActionResult> GetActualRevenue(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var revenue = await _revenueService.GetActualRevenueAsync(from, to);
        return Ok(revenue);
    }

    [HttpGet("actual/product/{softwareId:long}")]
    public async Task<IActionResult> GetActualRevenueForProduct(
        long softwareId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var revenue = await _revenueService.GetActualRevenueForProductAsync(
            softwareId, from, to);
        return Ok(revenue);
    }

    [HttpGet("predicted")]
    public async Task<IActionResult> GetPredictedRevenue()
    {
        var revenue = await _revenueService.GetPredictedRevenueAsync();
        return Ok(revenue);
    }

    [HttpGet("predicted/product/{softwareId:long}")]
    public async Task<IActionResult> GetPredictedRevenueForProduct(long softwareId)
    {
        var revenue = await _revenueService.GetPredictedRevenueForProductAsync(softwareId);
        return Ok(revenue);
    }

    [HttpGet("convert")]
    public async Task<IActionResult> ConvertPln(
        [FromQuery] decimal amountPln,
        [FromQuery] string currency)
    {
        var converted = await _revenueService.ConvertPlnToAsync(amountPln, currency);
        return Ok(new
        {
            amountPln,
            currency,
            converted
        });
    }
}