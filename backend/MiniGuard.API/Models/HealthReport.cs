namespace MiniGuard.API.Models;

public class HealthReport
{
    public string OverallSeverity { get; set; } = "HEALTHY";
    public List<ServiceHealth> AffectedServices { get; set; } = new();
    public string AiDiagnosis { get; set; } = string.Empty;
    public string RecommendedAction { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ServiceHealth
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "HEALTHY";
    public int ErrorCount { get; set; }
}
