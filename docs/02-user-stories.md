# User Stories
## MiniGuard AI — Production Anomaly Detection Agent

---

## Personas

| Persona | Role | Goal |
|---|---|---|
| **Alex** | Operations Engineer | Detect and respond to production failures fast |
| **Dev** | Software Developer | Understand which service is failing and why |
| **Sam** | Engineering Manager | Get confidence that production is healthy without asking the team |

---

## US-01 — Live Log Monitoring

**As an** Operations Engineer,
**I want** the system to continuously read and monitor application log files,
**So that** I don't have to manually tail logs or watch multiple terminals.

### Acceptance Criteria
1. The API reads `logs/app.log` automatically without any manual trigger
2. Log reading occurs at least once every 60 seconds
3. The last 100 log lines are used for each analysis cycle
4. Log monitoring continues running after the API starts — no manual restart needed
5. If the log file doesn't exist yet, the system waits gracefully and retries

---

## US-02 — Automated Anomaly Detection

**As an** Operations Engineer,
**I want** the system to automatically detect when errors and anomalies appear in the logs,
**So that** I am alerted within one scan cycle — not hours later via a customer complaint.

### Acceptance Criteria
1. The system sends the last 100 log lines to Claude AI for analysis every 60 seconds
2. Claude returns a structured JSON result with overall severity (HEALTHY / WARNING / CRITICAL)
3. Each service is assessed individually with its own status and error count
4. A plain-English diagnosis and recommended action is included in every result
5. If Claude API is unavailable, the system logs the failure and retries on the next cycle

---

## US-03 — Live Dashboard View

**As an** Engineering Manager,
**I want** to see a real-time dashboard showing the health of all production services,
**So that** I can verify system health at a glance without involving developers.

### Acceptance Criteria
1. Dashboard is accessible at `http://localhost:4200` in a browser
2. An overall status banner (green/amber/red) is prominently displayed
3. Each service (OrdersAPI, PaymentService, JobWorker, DBWorker, AuthService) has a health row
4. Each row shows: service name, status badge, error count, and error rate bar
5. Dashboard updates automatically when a new scan completes — no page refresh needed

---

## US-04 — Manual Scan Trigger

**As a** Software Developer,
**I want** to trigger an immediate anomaly scan on demand,
**So that** I can get an instant diagnosis after deploying a fix or investigating an incident.

### Acceptance Criteria
1. A "SCAN NOW" button is visible in the dashboard header at all times
2. Clicking it triggers `POST /api/scan` immediately
3. A loading spinner appears and the button is disabled while the scan runs
4. Results update on the dashboard within 10 seconds
5. The button returns to its normal state once results are received

---

## US-05 — Service Health Status Display

**As a** Software Developer,
**I want** to see the health status of each individual service clearly,
**So that** I can immediately identify which specific service is causing problems.

### Acceptance Criteria
1. Each of the 5 services has its own row in the service health table
2. Status badge is colour-coded: green = HEALTHY, amber = WARNING, red = CRITICAL
3. Rows with CRITICAL status visually pulse or highlight to draw attention
4. Error count is shown per service
5. The AI diagnosis panel explains which services are affected in plain English

---

## US-06 — Real-Time SignalR Updates

**As an** Operations Engineer,
**I want** the dashboard to update in real-time without me having to refresh the page,
**So that** I can leave it open on a monitor and trust it reflects the current state.

### Acceptance Criteria
1. The Angular dashboard connects to the SignalR hub at `/monitorhub` on load
2. When the API completes a scan, it pushes the result via SignalR immediately
3. All dashboard sections update reactively within 2 seconds of the scan completing
4. If the SignalR connection drops, the client attempts to reconnect automatically
5. A connection status indicator is visible in the dashboard header

---

## US-07 — Live Log Feed

**As a** Software Developer,
**I want** to see the most recent log entries displayed on the dashboard,
**So that** I can correlate AI diagnosis results with the raw log evidence.

### Acceptance Criteria
1. The last 20 log lines are displayed in a scrollable log feed panel
2. Log entries are colour-coded: red = ERROR/CRITICAL, amber = WARN, grey = INFO
3. The log feed auto-scrolls to the latest entry when new logs arrive
4. Log feed uses a monospace font for readability
5. Log feed is refreshed with each dashboard update cycle

---

*Document created as part of MiniGuard AI SDLC — Phase 0: User Stories*
*Date: 2026-03-12*
