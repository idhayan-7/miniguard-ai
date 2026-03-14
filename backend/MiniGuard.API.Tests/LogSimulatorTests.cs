using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using MiniGuard.API.Services;
using Xunit;

namespace MiniGuard.API.Tests;

public class LogSimulatorTests : IDisposable
{
    private readonly string _tempLog;
    private readonly LogSimulator _simulator;

    public LogSimulatorTests()
    {
        _tempLog = Path.Combine(Path.GetTempPath(), $"miniguard-test-{Guid.NewGuid()}.log");

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["LogFilePath"] = _tempLog,
                ["LogMaxLines"] = "500"
            })
            .Build();

        _simulator = new LogSimulator(config, NullLogger<LogSimulator>.Instance);
    }

    public void Dispose()
    {
        if (File.Exists(_tempLog))
            File.Delete(_tempLog);
    }

    // TC-LOG-01
    [Fact]
    public void WriteSingleEntry_HasCorrectFormat()
    {
        // Arrange & Act
        _simulator.WriteSingleEntry("INFO", "OrdersAPI", "GET /api/orders/123 - 200 OK - 45ms");

        // Assert
        var lines = File.ReadAllLines(_tempLog);
        Assert.Single(lines);
        Assert.Matches(
            @"^\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\] \[INFO\] OrdersAPI - GET /api/orders/123 - 200 OK - 45ms$",
            lines[0]);
    }

    // TC-LOG-02
    [Fact]
    public void ApplyRotation_KeepsMaxLines_WhenExceeded()
    {
        // Arrange
        var lines = Enumerable.Range(1, 600).Select(i => $"line {i}");
        File.WriteAllLines(_tempLog, lines);

        // Act
        _simulator.ApplyRotation(_tempLog, 500);

        // Assert
        var result = File.ReadAllLines(_tempLog);
        Assert.Equal(500, result.Length);
        Assert.Equal("line 101", result[0]);   // first 100 trimmed
        Assert.Equal("line 600", result[^1]);  // last line preserved
    }

    // TC-LOG-02 edge
    [Fact]
    public void ApplyRotation_DoesNothing_WhenUnderLimit()
    {
        // Arrange
        File.WriteAllLines(_tempLog, ["line 1", "line 2", "line 3"]);

        // Act
        _simulator.ApplyRotation(_tempLog, 500);

        // Assert
        Assert.Equal(3, File.ReadAllLines(_tempLog).Length);
    }

    // TC-LOG-03
    [Fact]
    public void GenerateInfoEntry_ContainsKnownServiceName()
    {
        // Arrange
        var knownServices = new[] { "OrdersAPI", "PaymentService", "JobWorker", "DBWorker", "AuthService" };

        // Act — generate 20 entries to cover random selection
        for (int i = 0; i < 20; i++)
        {
            var entry = _simulator.GenerateInfoEntry();

            // Assert format: [date] [INFO] ServiceName - message
            Assert.Matches(@"^\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\] \[INFO\] \w+ - .+$", entry);

            var serviceName = entry.Split("] [INFO] ")[1].Split(" - ")[0];
            Assert.Contains(serviceName, knownServices);
        }
    }

    // TC-LOG-04
    [Fact]
    public void InjectAnomalyBurst_Produces12To15ErrorEntries()
    {
        // Arrange
        File.WriteAllText(_tempLog, string.Empty);
        var linesBefore = File.ReadAllLines(_tempLog).Length;

        // Act
        _simulator.InjectAnomalyBurst();

        // Assert
        var linesAfter = File.ReadAllLines(_tempLog).Length;
        var burstCount = linesAfter - linesBefore;
        Assert.InRange(burstCount, 12, 15);
    }

    [Fact]
    public void InjectAnomalyBurst_OnlyWritesErrorOrCriticalEntries()
    {
        // Arrange
        File.WriteAllText(_tempLog, string.Empty);

        // Act
        _simulator.InjectAnomalyBurst();

        // Assert
        var lines = File.ReadAllLines(_tempLog);
        Assert.All(lines, line =>
            Assert.True(line.Contains("[ERROR]") || line.Contains("[CRITICAL]"),
                $"Expected ERROR or CRITICAL but got: {line}"));
    }

    [Fact]
    public void InjectAnomalyBurst_OnlyAffectsOrdersApiAndDbWorker()
    {
        // Arrange
        File.WriteAllText(_tempLog, string.Empty);

        // Act
        _simulator.InjectAnomalyBurst();

        // Assert
        var lines = File.ReadAllLines(_tempLog);
        Assert.All(lines, line =>
            Assert.True(line.Contains("OrdersAPI") || line.Contains("DBWorker"),
                $"Unexpected service in burst: {line}"));
    }
}
