using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MiniGuard.API.Services;

public class LogSimulator : ILogSimulator, IHostedService, IDisposable
{
    private readonly IConfiguration _config;
    private readonly ILogger<LogSimulator> _logger;
    private readonly string _logPath;
    private readonly int _maxLines;

    private Timer? _writeTimer;
    private Timer? _burstTimer;

    private static readonly string[] Services =
        ["OrdersAPI", "PaymentService", "JobWorker", "DBWorker", "AuthService"];

    private static readonly string[] InfoTemplates =
    [
        "GET /api/orders/{0} - 200 OK - {1}ms",
        "POST /api/payment - 200 OK - {1}ms",
        "EmailNotificationJob completed - {0} records processed",
        "User authentication successful - userId: {0}",
        "Database health check passed - {1}ms response",
        "GET /api/products - 200 OK - {1}ms",
        "Job batch processed - {0} items completed",
        "Cache refresh completed - {0} entries updated",
        "Scheduled task completed successfully - duration: {1}ms",
        "Health check passed - all dependencies reachable",
    ];

    private static readonly string[] ErrorTemplates =
    [
        "POST /api/payment - 500 InternalServerError - SqlException: Timeout expired",
        "Connection pool exhausted - waiting for available connection",
        "SQL Server unreachable - retry 3/3 failed",
        "CRITICAL: Transaction rollback initiated - data integrity risk",
        "GET /api/orders/{0} - 503 ServiceUnavailable - upstream timeout",
        "Unhandled exception in worker thread - System.OutOfMemoryException",
        "Database deadlock detected - query killed after 30s",
        "Connection string invalid or server not responding",
        "CRITICAL: Failed to acquire distributed lock - service degraded",
        "Batch job failed - 0/{0} records processed - rolling back",
        "HTTP client timeout after 10000ms - downstream service unresponsive",
        "Authentication service unreachable - fallback to cached tokens",
        "CRITICAL: Disk I/O error writing to temp storage",
        "Memory threshold exceeded - GC pressure high",
        "Queue consumer crashed - messages unprocessed",
    ];

    private readonly Random _rng = new();

    public LogSimulator(IConfiguration config, ILogger<LogSimulator> logger)
    {
        _config = config;
        _logger = logger;
        _logPath = config["LogFilePath"] ?? "logs/app.log";
        _maxLines = config.GetValue<int>("LogMaxLines", 500);
    }

    // --- IHostedService ---

    public Task StartAsync(CancellationToken cancellationToken)
    {
        EnsureLogDirectory();
        _logger.LogInformation("LogSimulator started. Writing to {Path}", _logPath);

        // Write a log entry every 2 seconds
        _writeTimer = new Timer(_ => WriteInfoEntry(), null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

        // Inject anomaly burst every 3 minutes
        _burstTimer = new Timer(_ => InjectAnomalyBurst(), null, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _writeTimer?.Change(Timeout.Infinite, 0);
        _burstTimer?.Change(Timeout.Infinite, 0);
        _logger.LogInformation("LogSimulator stopped.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _writeTimer?.Dispose();
        _burstTimer?.Dispose();
    }

    // --- ILogSimulator ---

    public void WriteSingleEntry(string level, string service, string message)
    {
        var entry = FormatEntry(level, service, message);
        AppendAndRotate(entry);
    }

    public void ApplyRotation(string logPath, int maxLines)
    {
        if (!File.Exists(logPath)) return;

        var lines = File.ReadAllLines(logPath);
        if (lines.Length <= maxLines) return;

        var trimmed = lines.TakeLast(maxLines).ToArray();
        File.WriteAllLines(logPath, trimmed);
    }

    public void InjectAnomalyBurst()
    {
        // 12–15 consecutive ERROR/CRITICAL entries affecting OrdersAPI + DBWorker
        var burstCount = _rng.Next(12, 16);
        var affectedServices = new[] { "OrdersAPI", "DBWorker" };

        for (int i = 0; i < burstCount; i++)
        {
            var service = affectedServices[_rng.Next(affectedServices.Length)];
            var level = i % 4 == 0 ? "CRITICAL" : "ERROR";
            var template = ErrorTemplates[_rng.Next(ErrorTemplates.Length)];
            var message = string.Format(template, _rng.Next(100, 999), _rng.Next(1000, 9999));
            var entry = FormatEntry(level, service, message);
            AppendAndRotate(entry);
        }

        _logger.LogWarning("Anomaly burst injected — {Count} error entries written", burstCount);
    }

    public string GenerateInfoEntry()
    {
        var service = Services[_rng.Next(Services.Length)];
        var template = InfoTemplates[_rng.Next(InfoTemplates.Length)];
        var message = string.Format(template, _rng.Next(100, 9999), _rng.Next(10, 500));
        return FormatEntry("INFO", service, message);
    }

    public string GenerateErrorEntry()
    {
        var service = Services[_rng.Next(Services.Length)];
        var template = ErrorTemplates[_rng.Next(ErrorTemplates.Length)];
        var message = string.Format(template, _rng.Next(100, 999), _rng.Next(1000, 9999));
        return FormatEntry("ERROR", service, message);
    }

    // --- Private helpers ---

    private void WriteInfoEntry()
    {
        var entry = GenerateInfoEntry();
        AppendAndRotate(entry);
    }

    private void AppendAndRotate(string entry)
    {
        try
        {
            File.AppendAllText(_logPath, entry + Environment.NewLine);
            ApplyRotation(_logPath, _maxLines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write log entry");
        }
    }

    private static string FormatEntry(string level, string service, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return $"[{timestamp}] [{level}] {service} - {message}";
    }

    private void EnsureLogDirectory()
    {
        var dir = Path.GetDirectoryName(_logPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }
}
