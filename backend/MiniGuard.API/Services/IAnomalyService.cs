using MiniGuard.API.Models;

namespace MiniGuard.API.Services;

public interface IAnomalyService
{
    Task<HealthReport?> ScanAsync();
    HealthReport? GetLastResult();
}
