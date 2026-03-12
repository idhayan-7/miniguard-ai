# CLAUDE.md — MiniGuard AI

> This is the project brain. Read this at the start of every session.
> Update this file after every phase is completed.

---

## Project Purpose

**MiniGuard AI** is a Production Anomaly Detection Agent built as an AI adoption demo.

**Problem:** A large-scale production system (.NET APIs + Angular + SQL Server + 10+ services)
has zero visibility. Failures are discovered only after data is corrupted or customers complain.

**Solution:** An AI agent that monitors service logs every 60 seconds, uses Claude AI to detect
anomalies, and displays live health status on an Angular dashboard via SignalR.

---

## Tech Stack

| Layer | Technology | Version |
|---|---|---|
| Backend API | ASP.NET Core | .NET 8 |
| Testing | xUnit | Latest |
| Frontend | Angular | 16.2.16 |
| UI Components | Angular Material | 16.x |
| Real-time | SignalR | ASP.NET Core built-in |
| AI Analysis | Claude API | claude-sonnet-4-20250514 |
| Node | Node.js | 18.20.8 |
| npm | npm | 10.8.2 |
| Angular CLI | @angular/cli | 16.2.16 |
| Version Control | Git | 2.41 |

---

## Architecture

```
[LogSimulator (IHostedService)]
    ↓ writes every 2s
logs/app.log
    ↓ reads last 100 lines every 60s
[AnomalyService (background timer)]
    ↓ sends to Claude API
[Claude API — claude-sonnet-4-20250514]
    ↓ returns JSON HealthReport
[AnomalyService]
    ↓ pushes via SignalR
[MonitorHub — /monitorhub]
    ↓ ReceiveHealthUpdate event
[Angular Dashboard — localhost:4200]
    — StatusBanner, ServiceHealthTable, AiDiagnosis, LogFeed
```

---

## Configuration Values (never change)

| Setting | Value |
|---|---|
| API Port | localhost:5000 |
| Angular Port | localhost:4200 |
| SignalR Hub Path | /monitorhub |
| SignalR Client Method | ReceiveHealthUpdate |
| Log File Path | logs/app.log |
| Log Max Lines | 500 (rolling) |
| Lines Sent to Claude | 100 (last 100) |
| Scan Interval | 60 seconds |
| Claude Model | claude-sonnet-4-20250514 |
| API Key Config Key | ClaudeApiKey |
| API Key File | appsettings.Development.json (gitignored) |

---

## Services Being Simulated

- OrdersAPI
- PaymentService
- JobWorker
- DBWorker
- AuthService

---

## Coding Rules

1. **Interface-driven services** — every service class must have a matching `IServiceName` interface
2. **No hardcoded values** — all config (ports, paths, keys, intervals) must be in `appsettings.json`
3. **Standalone Angular components** — use `standalone: true` throughout, Angular 16 style
4. **Arrange-Act-Assert** — all xUnit and Jasmine tests must follow AAA pattern
5. **TDD** — write failing tests BEFORE implementation code
6. **No secrets in git** — `appsettings.Development.json` must be in `.gitignore`

---

## Project Structure

```
D:\miniguard-ai\
├── CLAUDE.md                              ← This file
├── run-miniguard.bat                      ← One-click launcher
├── MiniGuard.sln
├── docs\
│   ├── 01-business-understanding.md
│   ├── 02-user-stories.md
│   ├── 03-task-breakdown.md
│   ├── 04-test-cases.md
│   ├── 05-code-review-log.md              ← Phase 3
│   ├── 06-api-docs.md                     ← Phase 6
│   ├── 07-setup-guide.md                  ← Phase 6
│   ├── 08-user-guide.md                   ← Phase 6
│   ├── 09-ado-work-items.json             ← Phase 6
│   └── 10-sprint-plan.md                  ← Phase 6
├── MiniGuard.API\
│   ├── Controllers\HealthController.cs
│   ├── Hubs\MonitorHub.cs
│   ├── Models\HealthReport.cs
│   ├── Services\
│   │   ├── ILogSimulator.cs
│   │   ├── LogSimulator.cs
│   │   ├── IAnomalyService.cs
│   │   └── AnomalyService.cs
│   ├── appsettings.json
│   └── appsettings.Development.json       ← gitignored, holds ClaudeApiKey
├── MiniGuard.API.Tests\
│   ├── LogSimulatorTests.cs
│   ├── AnomalyServiceTests.cs
│   └── HealthControllerTests.cs
└── MiniGuard.Web\
    └── src\app\
        ├── components\
        │   ├── header\
        │   ├── status-banner\
        │   ├── stats-row\
        │   ├── service-health\
        │   ├── ai-diagnosis\
        │   └── log-feed\
        ├── services\
        │   ├── monitor.service.ts
        │   └── signalr.service.ts
        └── environments\
            └── environment.ts              ← API URLs here
```

---

## Git Workflow

```
main                    ← always stable, merged into after each phase
feature/sdlc-docs
feature/phase-1-project-setup
feature/phase-2-log-simulator
feature/phase-3-claude-ai-analysis
feature/phase-4-angular-dashboard
feature/phase-5-signalr-live-updates
feature/phase-6-demo-polish
```

Per-phase git flow:
```bash
git checkout main && git pull origin main
git checkout -b feature/phase-N-name
# ... build ...
git add .
git commit -m "phase-N: description"
git push origin feature/phase-N-name
git checkout main
git merge feature/phase-N-name
git push origin main
```

---

## Phase Checklist

- [x] SDLC Docs — business understanding, user stories, tasks, test stubs
- [ ] Phase 1 — Project structure: MiniGuard.API + MiniGuard.API.Tests + MiniGuard.Web + solution file
- [ ] Phase 2 — Log Simulator: ILogSimulator, LogSimulator IHostedService, anomaly burst
- [ ] Phase 3 — Claude AI Analysis: IAnomalyService, AnomalyService, MonitorHub, endpoints
- [ ] Phase 4 — Angular Dashboard: all components, dark theme, Material
- [ ] Phase 5 — Wire Together: end-to-end SignalR, CORS, run-miniguard.bat
- [ ] Phase 6 — Demo Polish: animations, sparkline, final docs

---

## Current Status

**Date:** 2026-03-12
**Branch:** feature/sdlc-docs (merging to main after commit)
**Completed:** SDLC documentation — all 4 docs created, 3 xUnit test stubs created
**Next session:** Start Phase 1 — scaffold MiniGuard.API (.NET 8), MiniGuard.API.Tests (xUnit), MiniGuard.Web (Angular 16 standalone), solution file, run-miniguard.bat
**Known issues:** None
**Blockers:** Claude API key needed before Phase 3 — paste into appsettings.Development.json

---

## Build & Run Commands (filled in as phases complete)

```bash
# Phase 1+: Build API
cd MiniGuard.API && dotnet build

# Phase 1+: Run API
cd MiniGuard.API && dotnet run

# Phase 1+: Run Tests
cd MiniGuard.API.Tests && dotnet test

# Phase 1+: Serve Angular
cd MiniGuard.Web && ng serve

# Phase 5+: Launch everything
run-miniguard.bat
```
