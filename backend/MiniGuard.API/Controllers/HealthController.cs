using Microsoft.AspNetCore.Mvc;
using MiniGuard.API.Services;

namespace MiniGuard.API.Controllers;

[ApiController]
[Route("api")]
public class HealthController : ControllerBase
{
    private readonly IAnomalyService _anomalyService;

    public HealthController(IAnomalyService anomalyService)
    {
        _anomalyService = anomalyService;
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        var result = _anomalyService.GetLastResult();
        if (result is null)
            return NoContent();
        return Ok(result);
    }

    [HttpPost("scan")]
    public async Task<IActionResult> TriggerScan()
    {
        var result = await _anomalyService.ScanAsync();
        if (result is null)
            return StatusCode(503, new { message = "Scan failed — check API key and log file." });
        return Ok(result);
    }

    [HttpGet("logs")]
    public IActionResult GetLogs([FromServices] IConfiguration config)
    {
        var logPath = config["LogFilePath"] ?? "logs/app.log";
        if (!System.IO.File.Exists(logPath))
            return Ok(Array.Empty<string>());

        var lines = System.IO.File.ReadAllLines(logPath);
        var last50 = lines.TakeLast(50).ToArray();
        return Ok(last50);
    }
}
