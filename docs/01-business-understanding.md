# Business Understanding Document
## MiniGuard AI — Production Anomaly Detection Agent

---

## 1. Problem Statement

Our large-scale production system operates across 10+ services including:
- **OrdersAPI** — customer-facing order processing
- **PaymentService** — payment gateway integration
- **JobWorker** — background processing jobs
- **DBWorker** — database maintenance and sync workers
- **AuthService** — authentication and session management

Currently the system has **zero automated monitoring visibility**. Failures are discovered only after:
- Customers report data corruption or failed transactions
- Support tickets escalate from end-users
- DBAs notice inconsistencies during manual checks

This reactive approach means problems exist undetected for minutes to hours before anyone acts.

---

## 2. Business Impact

| Impact Area | Current State | Cost |
|---|---|---|
| Detection latency | Manual — customers report first | Hours of undetected downtime |
| Data integrity | Corrupted records discovered late | Rollback costs, customer refunds |
| Operational overhead | Engineers on constant alert | Developer burnout, reactive culture |
| Customer trust | Failures visible externally first | Reputational damage |
| Incident response | No automated alerts | Slow mean-time-to-resolution (MTTR) |

**Estimated risk:** A single undetected SQL Server connection pool exhaustion event affecting OrdersAPI and DBWorker can result in failed orders, corrupted batch jobs, and 30–60 minutes of manual triage.

---

## 3. Proposed Solution

**MiniGuard AI** — a lightweight, AI-powered production anomaly detection agent that:

1. **Monitors** structured log output from all services continuously
2. **Detects** error patterns, spikes, and anomalies using Claude AI analysis
3. **Reports** health status per service (HEALTHY / WARNING / CRITICAL)
4. **Displays** results on a live Angular dashboard with real-time SignalR updates
5. **Alerts** engineers immediately when anomalies are detected

**Architecture:**
```
[Services write logs] → logs/app.log
        ↓
[MiniGuard.API] reads logs every 60s
        ↓
[Claude AI] analyses last 100 lines → returns JSON health report
        ↓
[SignalR] pushes update to dashboard
        ↓
[Angular Dashboard] displays live service health + AI diagnosis
```

---

## 4. Success Criteria

| # | Criterion | Measure |
|---|---|---|
| SC-1 | Anomalies detected within one scan cycle | ≤ 60 seconds from first error log |
| SC-2 | Per-service health status visible on dashboard | All 5 services shown with status badge |
| SC-3 | AI diagnosis is human-readable | Plain English explanation + recommended action |
| SC-4 | Manual scan can be triggered on demand | SCAN NOW button returns result within 10s |
| SC-5 | Dashboard updates in real-time | SignalR pushes update within 2s of scan completing |
| SC-6 | System runs unattended | Background timer runs 24/7 without manual intervention |

---

## 5. Assumptions

- Log format is structured: `[YYYY-MM-DD HH:mm:ss] [LEVEL] ServiceName - Message`
- All 5 services write to a single shared log file: `logs/app.log`
- Claude API (`claude-sonnet-4-20250514`) is accessible via API key
- API runs on `localhost:5000`, Angular on `localhost:4200` (demo environment)
- This is a **demo** — not connected to real production logs in Phase 1

---

## 6. Out of Scope (Phase 1)

- Integration with real IIS or SQL Server logs (Phase 2 target)
- Email / Teams / PagerDuty alerting (can be added post-demo)
- Authentication or multi-user access to the dashboard
- Historical trend analysis or data persistence (no database in Phase 1)
- Deployment to cloud or production infrastructure
- Multi-environment support (dev/staging/prod)

---

*Document created as part of MiniGuard AI SDLC — Phase 0: Business Understanding*
*Date: 2026-03-12*
