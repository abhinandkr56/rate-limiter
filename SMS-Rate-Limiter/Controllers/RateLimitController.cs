using Microsoft.AspNetCore.Mvc;
using SMS_Rate_Limiter.Services;

namespace SMS_Rate_Limiter.Controllers;

[ApiController]
[Route("api/rate")]
public class RateLimitController : ControllerBase
{
    private readonly IRateLimitService _rateService;

    public RateLimitController(IRateLimitService rateService)
    {
        _rateService = rateService;
    }

    [HttpGet("can-send")]
    public IActionResult CanSend([FromQuery] string accountId, [FromQuery] string businessPhone)
    {
        return Ok(new { allowed = _rateService.CanSend(accountId, businessPhone) });
    }
}