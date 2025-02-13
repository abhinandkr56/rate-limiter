using Microsoft.AspNetCore.Mvc;
using SMS_Rate_Limiter.Services;

namespace SMS_Rate_Limiter.Controllers;
[ApiController]
[Route("api/monitoring")]
public class MonitoringController : ControllerBase
{
    private readonly IMongoService _mongoService;

    public MonitoringController(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }

    [HttpGet("accounts")]
    public IActionResult GetAccounts()
    {
        var accounts = _mongoService.GetAllAccounts();
        return Ok(accounts);
    }

    [HttpGet("business-numbers")]
    public IActionResult GetBusinessNumbers([FromQuery] string accountId)
    {
        var businessNumbers = _mongoService.GetBusinessNumbers(accountId);
        return Ok(businessNumbers);
    }

    [HttpGet("history")]
    public IActionResult GetHistory(
        [FromQuery] string? accountId,
        [FromQuery] string? businessPhone,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end)
    {
        var history = _mongoService.GetHistory(accountId, businessPhone, start, end);
        return Ok(history);
    }
}