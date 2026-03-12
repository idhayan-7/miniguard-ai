# Test Cases
## MiniGuard AI — Production Anomaly Detection Agent

> Strategy: TDD — all tests written BEFORE implementation code.
> .NET tests use xUnit with Arrange-Act-Assert pattern.
> Angular tests use Jasmine with TestBed.

---

## .NET Unit Tests — MiniGuard.API.Tests

### TC-LOG-01: Log entry format is correct
- **File:** `LogSimulatorTests.cs`
- **Method:** `LogEntry_HasCorrectFormat_WhenWritten`
- **Arrange:** Instantiate LogSimulator with temp log path from config
- **Act:** Trigger one write cycle
- **Assert:** Log line matches pattern `[YYYY-MM-DD HH:mm:ss] [LEVEL] ServiceName - Message`

### TC-LOG-02: Log rotation keeps max 500 lines
- **File:** `LogSimulatorTests.cs`
- **Method:** `LogFile_KeepsMax500Lines_AfterExceeding`
- **Arrange:** Write 600 log lines to temp file
- **Act:** Call rotation method
- **Assert:** File contains exactly 500 lines

### TC-LOG-03: INFO entries contain known service names
- **File:** `LogSimulatorTests.cs`
- **Method:** `InfoEntry_ContainsKnownServiceName`
- **Arrange:** Generate INFO log entry
- **Act:** Parse service name field
- **Assert:** Service name is one of: OrdersAPI, PaymentService, JobWorker, DBWorker, AuthService

### TC-LOG-04: Anomaly burst produces 12–15 error entries
- **File:** `LogSimulatorTests.cs`
- **Method:** `AnomalyBurst_Produces12To15ErrorEntries`
- **Arrange:** Trigger anomaly burst method directly
- **Act:** Count ERROR/CRITICAL lines written
- **Assert:** Count is between 12 and 15 inclusive

---

### TC-SCAN-01: ScanAsync returns valid HealthReport
- **File:** `AnomalyServiceTests.cs`
- **Method:** `ScanAsync_ReturnsValidHealthReport_WhenClaudeResponds`
- **Arrange:** Mock `HttpClient` to return valid JSON Claude response
- **Act:** Call `ScanAsync()`
- **Assert:** `HealthReport` is not null, `OverallSeverity` is one of HEALTHY/WARNING/CRITICAL

### TC-SCAN-02: ScanAsync handles Claude API failure gracefully
- **File:** `AnomalyServiceTests.cs`
- **Method:** `ScanAsync_ReturnsNull_WhenClaudeApiThrows`
- **Arrange:** Mock `HttpClient` to throw `HttpRequestException`
- **Act:** Call `ScanAsync()`
- **Assert:** Returns null (or default), does not throw unhandled exception

### TC-SCAN-03: ScanAsync reads last 100 lines from log file
- **File:** `AnomalyServiceTests.cs`
- **Method:** `ScanAsync_ReadsLast100Lines_FromLogFile`
- **Arrange:** Write 200 lines to temp log, inject path via config
- **Act:** Call `ScanAsync()` (intercept HTTP request body)
- **Assert:** Request body contains exactly 100 log lines

### TC-SCAN-04: HealthReport deserialises affectedServices correctly
- **File:** `AnomalyServiceTests.cs`
- **Method:** `HealthReport_DeserializesAffectedServices_Correctly`
- **Arrange:** Mock Claude response with 3 affected services in JSON
- **Act:** Call `ScanAsync()`, inspect returned `HealthReport`
- **Assert:** `AffectedServices` count is 3, each has Name, Status, ErrorCount

### TC-SCAN-05: ClaudeApiKey is read from configuration, not hardcoded
- **File:** `AnomalyServiceTests.cs`
- **Method:** `AnomalyService_ReadsApiKey_FromConfiguration`
- **Arrange:** Set up `IConfiguration` with test key value
- **Act:** Instantiate `AnomalyService`, capture outgoing HTTP request header
- **Assert:** Authorization header matches configured key, not a literal string

---

### TC-API-01: GET /api/health returns 200 with HealthReport
- **File:** `HealthControllerTests.cs`
- **Method:** `GetHealth_Returns200_WithHealthReport`
- **Arrange:** Mock `IAnomalyService` returning a pre-built `HealthReport`
- **Act:** Call controller `GetHealth()` action
- **Assert:** Result is `OkObjectResult`, body is a `HealthReport`

### TC-API-02: GET /api/health returns 204 when no scan has run yet
- **File:** `HealthControllerTests.cs`
- **Method:** `GetHealth_Returns204_WhenNoScanCompleted`
- **Arrange:** Mock `IAnomalyService` returning null
- **Act:** Call controller `GetHealth()` action
- **Assert:** Result is `NoContentResult`

### TC-API-03: POST /api/scan triggers ScanAsync and returns result
- **File:** `HealthControllerTests.cs`
- **Method:** `PostScan_TriggersAnomalyService_AndReturnsReport`
- **Arrange:** Mock `IAnomalyService.ScanAsync()` returning a HealthReport
- **Act:** Call controller `TriggerScan()` action
- **Assert:** `ScanAsync` was called exactly once; result is `OkObjectResult`

### TC-API-04: GET /api/logs returns last 50 lines
- **File:** `HealthControllerTests.cs`
- **Method:** `GetLogs_ReturnsLast50Lines`
- **Arrange:** Write 100 lines to temp log file; inject path via config
- **Act:** Call controller `GetLogs()` action
- **Assert:** Response body contains array of exactly 50 strings

---

## Angular Tests — Jasmine / TestBed

### TC-ANG-01: HeaderComponent renders title and LIVE indicator
- **File:** `header.component.spec.ts`
- **Assert:** DOM contains "MiniGuard AI" text and `.live-indicator` element

### TC-ANG-02: StatusBannerComponent applies correct CSS class per severity
- **File:** `status-banner.component.spec.ts`
- **Assert:** HEALTHY → `.banner-healthy`, WARNING → `.banner-warning`, CRITICAL → `.banner-critical`

### TC-ANG-03: ServiceHealthComponent renders one row per service
- **File:** `service-health.component.spec.ts`
- **Assert:** Given mock data with 5 services, table has 5 data rows

### TC-ANG-04: ServiceHealthComponent applies CRITICAL pulse class
- **File:** `service-health.component.spec.ts`
- **Assert:** Row with status=CRITICAL has `.pulse-critical` CSS class

### TC-ANG-05: MonitorService calls correct API endpoints
- **File:** `monitor.service.spec.ts`
- **Assert:** `triggerScan()` calls `POST /api/scan`; `getHealth()` calls `GET /api/health`

### TC-ANG-06: SignalrService connects to hub URL from environment
- **File:** `signalr.service.spec.ts`
- **Assert:** HubConnectionBuilder is called with `environment.hubUrl`

### TC-ANG-07: LogFeedComponent colour-codes entries by level
- **File:** `log-feed.component.spec.ts`
- **Assert:** ERROR line has `.log-error` class; WARN has `.log-warn`; INFO has `.log-info`

---

## Test Coverage Targets

| Layer | Target Coverage |
|---|---|
| .NET Services | ≥ 80% line coverage |
| .NET Controllers | ≥ 90% line coverage |
| Angular Services | ≥ 80% branch coverage |
| Angular Components | ≥ 70% (DOM assertions) |

---

*Document created as part of MiniGuard AI SDLC — Phase 0: Test Cases*
*Date: 2026-03-12*
