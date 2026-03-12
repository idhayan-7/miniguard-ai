# Task Breakdown
## MiniGuard AI — Production Anomaly Detection Agent

---

## US-01 — Live Log Monitoring

| Task ID | Description | Est (hrs) | Dependencies | Files Affected |
|---|---|---|---|---|
| T-01-1 | Create `ILogSimulator` interface with `Start()` and `Stop()` methods | 0.5 | None | `MiniGuard.API/Services/ILogSimulator.cs` |
| T-01-2 | Implement `LogSimulator.cs` as `IHostedService` writing entries every 2s | 2.0 | T-01-1 | `MiniGuard.API/Services/LogSimulator.cs` |
| T-01-3 | Add log rotation — keep last 500 lines only | 0.5 | T-01-2 | `MiniGuard.API/Services/LogSimulator.cs` |
| T-01-4 | Add `LogFilePath` to `appsettings.json`, read via `IConfiguration` | 0.5 | T-01-2 | `MiniGuard.API/appsettings.json`, `LogSimulator.cs` |
| T-01-5 | Register `LogSimulator` as `IHostedService` in `Program.cs` | 0.25 | T-01-2 | `MiniGuard.API/Program.cs` |
| T-01-6 | Write xUnit tests for log entry format and rotation behaviour | 1.0 | T-01-2 | `MiniGuard.API.Tests/LogSimulatorTests.cs` |

**US-01 Total: 4.75 hrs**

---

## US-02 — Automated Anomaly Detection

| Task ID | Description | Est (hrs) | Dependencies | Files Affected |
|---|---|---|---|---|
| T-02-1 | Create `IAnomalyService` interface with `ScanAsync()` returning `HealthReport` | 0.5 | None | `MiniGuard.API/Services/IAnomalyService.cs` |
| T-02-2 | Create `HealthReport` model with `overallSeverity`, `affectedServices`, `aiDiagnosis`, etc. | 0.5 | None | `MiniGuard.API/Models/HealthReport.cs`, `ServiceHealth.cs` |
| T-02-3 | Implement `AnomalyService.cs` — reads last 100 log lines from `logs/app.log` | 1.0 | T-02-1, T-01-4 | `MiniGuard.API/Services/AnomalyService.cs` |
| T-02-4 | Add Claude API HTTP call using `HttpClient` — sends logs, parses JSON response | 2.0 | T-02-3 | `MiniGuard.API/Services/AnomalyService.cs` |
| T-02-5 | Add `ClaudeApiKey` and `ScanIntervalSeconds` to `appsettings.json` | 0.25 | T-02-4 | `MiniGuard.API/appsettings.json` |
| T-02-6 | Add `appsettings.Development.json` for API key secret (gitignored) | 0.25 | T-02-5 | `MiniGuard.API/appsettings.Development.json`, `.gitignore` |
| T-02-7 | Implement background timer in `AnomalyService` — scan every 60s automatically | 1.0 | T-02-4 | `MiniGuard.API/Services/AnomalyService.cs` |
| T-02-8 | Add error handling — log failure and skip cycle if Claude API unavailable | 0.5 | T-02-4 | `MiniGuard.API/Services/AnomalyService.cs` |
| T-02-9 | Write xUnit tests for `AnomalyService` — mock HTTP client, assert JSON parsing | 2.0 | T-02-4 | `MiniGuard.API.Tests/AnomalyServiceTests.cs` |

**US-02 Total: 8.0 hrs**

---

## US-03 — Live Dashboard View

| Task ID | Description | Est (hrs) | Dependencies | Files Affected |
|---|---|---|---|---|
| T-03-1 | Scaffold Angular 16 app: `ng new MiniGuard.Web --routing --style=scss --standalone` | 0.5 | None | `MiniGuard.Web/` (entire scaffold) |
| T-03-2 | Install Angular Material + configure dark theme | 0.5 | T-03-1 | `MiniGuard.Web/package.json`, `styles.scss`, `app.config.ts` |
| T-03-3 | Create `HeaderComponent` — title, LIVE indicator, timestamp, countdown | 1.5 | T-03-1 | `MiniGuard.Web/src/app/components/header/` |
| T-03-4 | Create `StatusBannerComponent` — full-width green/amber/red severity banner | 1.0 | T-03-1 | `MiniGuard.Web/src/app/components/status-banner/` |
| T-03-5 | Create `ServiceHealthComponent` — table with badge, error count, error rate bar | 2.0 | T-03-1 | `MiniGuard.Web/src/app/components/service-health/` |
| T-03-6 | Create `AiDiagnosisComponent` — diagnosis text + recommended action panel | 1.0 | T-03-1 | `MiniGuard.Web/src/app/components/ai-diagnosis/` |
| T-03-7 | Set API base URL in `environment.ts` — never hardcoded in components | 0.25 | T-03-1 | `MiniGuard.Web/src/environments/environment.ts` |
| T-03-8 | Write Jasmine tests for each component with mock data | 1.5 | T-03-3 to T-03-6 | `*.spec.ts` per component |

**US-03 Total: 8.25 hrs**

---

## US-04 — Manual Scan Trigger

| Task ID | Description | Est (hrs) | Dependencies | Files Affected |
|---|---|---|---|---|
| T-04-1 | Create `HealthController.cs` with `POST /api/scan` endpoint | 0.5 | T-02-1 | `MiniGuard.API/Controllers/HealthController.cs` |
| T-04-2 | Create `GET /api/health` endpoint returning latest scan result | 0.5 | T-04-1 | `MiniGuard.API/Controllers/HealthController.cs` |
| T-04-3 | Create `GET /api/logs` endpoint returning last 50 log lines | 0.5 | T-01-4 | `MiniGuard.API/Controllers/HealthController.cs` |
| T-04-4 | Add SCAN NOW button to `HeaderComponent` with spinner and disabled state | 1.0 | T-03-3 | `MiniGuard.Web/src/app/components/header/` |
| T-04-5 | Create `MonitorService` in Angular — wraps HTTP calls to API | 1.0 | T-03-7 | `MiniGuard.Web/src/app/services/monitor.service.ts` |
| T-04-6 | Write xUnit tests for `HealthController` endpoints | 1.0 | T-04-1 | `MiniGuard.API.Tests/HealthControllerTests.cs` |

**US-04 Total: 4.5 hrs**

---

## US-05 — Service Health Status Display

| Task ID | Description | Est (hrs) | Dependencies | Files Affected |
|---|---|---|---|---|
| T-05-1 | Add colour-coded status badges to `ServiceHealthComponent` | 0.5 | T-03-5 | `MiniGuard.Web/src/app/components/service-health/` |
| T-05-2 | Add pulsing animation for CRITICAL rows | 0.5 | T-05-1 | `service-health.component.scss` |
| T-05-3 | Add error rate progress bar per service row | 0.5 | T-03-5 | `MiniGuard.Web/src/app/components/service-health/` |
| T-05-4 | Add stats row — 4 Material cards (healthy count, warning count, critical count, AI status) | 1.0 | T-03-1 | `MiniGuard.Web/src/app/components/stats-row/` |

**US-05 Total: 2.5 hrs**

---

## US-06 — Real-Time SignalR Updates

| Task ID | Description | Est (hrs) | Dependencies | Files Affected |
|---|---|---|---|---|
| T-06-1 | Install SignalR NuGet package and configure hub in `Program.cs` | 0.5 | None | `MiniGuard.API/Program.cs`, `MiniGuard.API.csproj` |
| T-06-2 | Create `MonitorHub.cs` — SignalR hub class | 0.5 | T-06-1 | `MiniGuard.API/Hubs/MonitorHub.cs` |
| T-06-3 | Add CORS policy for `localhost:4200` in `Program.cs` | 0.25 | T-06-1 | `MiniGuard.API/Program.cs` |
| T-06-4 | Inject `IHubContext<MonitorHub>` into `AnomalyService` — push after each scan | 0.5 | T-06-2, T-02-7 | `MiniGuard.API/Services/AnomalyService.cs` |
| T-06-5 | Install `@aspnet/signalr` in Angular and create `SignalrService` | 1.0 | T-03-1 | `MiniGuard.Web/src/app/services/signalr.service.ts` |
| T-06-6 | Connect `SignalrService` to hub on app init — listen for `ReceiveHealthUpdate` | 1.0 | T-06-5 | `MiniGuard.Web/src/app/app.component.ts` |
| T-06-7 | Wire `ReceiveHealthUpdate` to update all dashboard components reactively | 1.0 | T-06-6 | All Angular components |
| T-06-8 | Add auto-reconnect logic to `SignalrService` | 0.5 | T-06-5 | `MiniGuard.Web/src/app/services/signalr.service.ts` |
| T-06-9 | Add connection status indicator to `HeaderComponent` | 0.5 | T-06-6 | `MiniGuard.Web/src/app/components/header/` |

**US-06 Total: 5.75 hrs**

---

## US-07 — Live Log Feed

| Task ID | Description | Est (hrs) | Dependencies | Files Affected |
|---|---|---|---|---|
| T-07-1 | Create `LogFeedComponent` — displays last 20 log lines | 1.0 | T-03-1 | `MiniGuard.Web/src/app/components/log-feed/` |
| T-07-2 | Add colour coding: red=ERROR/CRITICAL, amber=WARN, grey=INFO | 0.5 | T-07-1 | `log-feed.component.scss` |
| T-07-3 | Add auto-scroll to latest entry | 0.5 | T-07-1 | `log-feed.component.ts` |
| T-07-4 | Fetch logs from `GET /api/logs` and refresh with each scan cycle | 0.5 | T-04-3, T-07-1 | `MiniGuard.Web/src/app/services/monitor.service.ts` |

**US-07 Total: 2.5 hrs**

---

## Infrastructure / Setup Tasks

| Task ID | Description | Est (hrs) | Dependencies | Files Affected |
|---|---|---|---|---|
| T-INF-1 | Create `MiniGuard.sln` solution file linking API + Tests projects | 0.25 | None | `MiniGuard.sln` |
| T-INF-2 | Create `run-miniguard.bat` — launches API + Angular in separate terminals | 0.5 | T-03-1 | `run-miniguard.bat` |
| T-INF-3 | Create `.gitignore` — exclude `appsettings.Development.json`, `node_modules`, `bin`, `obj` | 0.25 | None | `.gitignore` |
| T-INF-4 | Enable Swagger in API for endpoint testing | 0.25 | T-04-1 | `MiniGuard.API/Program.cs` |

**Infrastructure Total: 1.25 hrs**

---

## Summary

| Phase | Stories Covered | Est Total |
|---|---|---|
| Phase 1 — Project Setup | INF tasks | 1.25 hrs |
| Phase 2 — Log Simulator | US-01 | 4.75 hrs |
| Phase 3 — Claude AI Analysis | US-02, US-04 (API side) | 10.5 hrs |
| Phase 4 — Angular Dashboard | US-03, US-05, US-07 | 13.25 hrs |
| Phase 5 — Wire Together | US-06 | 5.75 hrs |
| Phase 6 — Polish | Animations, sparkline, docs | ~4.0 hrs |
| **Grand Total** | | **~39.5 hrs** |

---

*Document created as part of MiniGuard AI SDLC — Phase 0: Task Breakdown*
*Date: 2026-03-12*
