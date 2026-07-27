# Feature: Metrics Monitoring (OpenTelemetry + Prometheus + Grafana)

## Goal

Add a **metrics-first** observability stack to this teaching sample so you can watch
all microservices from one place (Grafana), without turning the solution into a
full production observability platform.

- **Phase 1 (this feature):** Metrics only
- **Phase 2 (later):** Distributed traces (and optionally logs correlation)

Persistence is in-memory, so there is **no EF Core / SQL instrumentation** to
expose. We focus on what the sample already has: HTTP, gRPC, HttpClient, Kafka,
and a few simple business counters.

## Why this approach

| Option | Verdict for this sample |
|--------|-------------------------|
| Per-service `/metrics` + Prometheus scrape | **Chosen for Phase 1** — easy to learn, `curl`able, matches common .NET tutorials |
| OTLP push → OpenTelemetry Collector → Prometheus | Better for real production; add later if we want a “collector hub” lesson |
| Azure Monitor / App Insights only | Fine for Azure shops, but hides the open stack (Prometheus/Grafana) we want to teach |

**Standard exposure path (Phase 1):**

```
Each .NET service
  → OpenTelemetry MeterProvider
  → Prometheus exporter
  → HTTP GET /metrics   (Prometheus text format)
  → Prometheus scrapes every N seconds
  → Grafana queries Prometheus
```

## Target architecture

```
ApiGateway ──────────── /metrics ─┐
ProductService ──────── /metrics ─┼──► Prometheus ──► Grafana
CartService ─────────── /metrics ─┤
NotificationService ─── /metrics ─┘

(Traces / Tempo / Loki = Phase 2 — out of scope now)
```

Docker Compose grows from Kafka-only to:

| Container | Role | Local URL |
|-----------|------|-----------|
| `prometheus` | Scrapes `/metrics` from host services | http://localhost:9090 |
| `grafana` | Dashboards over Prometheus | http://localhost:3000 |

Apps still run with `dotnet run` on the host (same as today). Prometheus reaches
them via `host.docker.internal` (Windows/macOS Docker).

## What we already have vs what we need

| Area | Today | Needed for metrics |
|------|-------|--------------------|
| OpenTelemetry | None | Shared registration helper + packages |
| `/metrics` endpoint | None | `MapPrometheusScrapingEndpoint()` on each app |
| Health checks | None | Optional companion later; not required for metrics |
| EF Core DB metrics | N/A (in-memory) | Skip |
| ASP.NET / Kestrel / HttpClient | Built-in meters exist once OTel is wired | Enable AspNetCore + Runtime + HttpClient instrumentation |
| gRPC (ProductService) | Present | Covered partly by ASP.NET Core metrics; optional custom counters later |
| Kafka / MassTransit | Present | Start with process/HTTP metrics; optional business counters for publish/consume |
| Prometheus / Grafana | Not in Compose | Add services + scrape config + datasource provisioning |
| Business KPIs | None | Small custom meters (e.g. products created, cart lines added, notifications handled) |

## Design decisions

| Topic | Decision |
|-------|----------|
| Signal for Phase 1 | **Metrics only** — traces deferred |
| API standard | OpenTelemetry .NET (`System.Diagnostics.Metrics` + OTel SDK) |
| Export format | Prometheus scrape at `/metrics` |
| Shared wiring | `Company.Shared` extension e.g. `AddTelemetry()` so every service configures the same way |
| Resource attributes | `service.name` per project (`product-service`, `cart-service`, `api-gateway`, `notification-service`) |
| Auto instrumentation | AspNetCore, HttpClient, Runtime (CPU/GC/thread pool) |
| Custom metrics | Few teaching counters only — avoid high-cardinality labels (no user ids, no raw URLs) |
| EF Core / SQL | Skip until real persistence exists |
| Gateway metrics | Instrument ApiGateway too (edge latency/status codes matter) |
| NotificationService | Still a WebApplication — expose `/metrics` even without public REST |
| Dashboards | Minimal provisioned Grafana dashboard (request rate, errors, latency, runtime) |
| Secrets / auth on `/metrics` | None in sample (local learning only) |

## Golden signals we will show

1. **Traffic** — request rate per service (`http.server.request.duration` / ASP.NET meters)
2. **Errors** — 4xx/5xx (or fault) rates
3. **Latency** — p95 / histogram buckets
4. **Saturation** — runtime (GC, thread pool) as a light stand-in without real DB pool metrics

Plus 1–2 **business** counters so the sample is not “infra only”.

## Implementation tasks

### Phase 1A — Shared OpenTelemetry setup

- [x] Add OpenTelemetry packages to `Company.Shared` (Hosting, AspNetCore, Http, Runtime, Prometheus.AspNetCore)
- [x] Add `AddTelemetry(serviceName)` + `MapMetrics()` extensions
- [x] Document that each service owns its own `service.name` (e.g. `ProductMetrics.ServiceName`)

### Phase 1B — Wire every service

- [x] `Company.ProductService` — register telemetry + `MapPrometheusScrapingEndpoint()`
- [x] `Company.CartService` — same
- [x] `ApiGateway` — same
- [x] `NotificationService` — same (so consumers are visible)

### Phase 1C — Custom teaching metrics

- [x] Product created counter (ProductService)
- [x] Cart item added / cart viewed counters (CartService)
- [x] Notification handled counter (NotificationService)
- [x] Keep label cardinality low (e.g. outcome = success/failure only)

### Phase 1D — Infra in Docker Compose

- [x] Add `prometheus` + `prometheus.yml` scrape targets for host ports
- [x] Add `grafana` with Prometheus datasource provisioning
- [x] Dashboards created manually in Grafana (not checked into the repo)
- [x] Update README quick start (Compose brings Kafka + metrics stack)

### Phase 1E — Docs & verify

- [x] Update root `README.md` architecture + “what you can learn”
- [ ] Manual check: hit APIs → see series in Prometheus → panels in Grafana
- [x] Keep this file’s checkboxes in sync as work lands

### Phase 2 — Traces (later, not now)

- [ ] `WithTracing` + AspNetCore / HttpClient / gRPC instrumentation
- [ ] OTLP export to Grafana Tempo (or Jaeger)
- [ ] Trace ↔ metric exemplars if useful for teaching
- [ ] Optionally introduce OpenTelemetry Collector as the single export hub

## Suggested teaching order (when coding)

1. Explain Meter vs Counter vs Histogram (2 minutes)
2. Add Shared `AddTelemetry` and wire **one** service; `curl /metrics`
3. Add Prometheus scrape; show the same series in Prometheus UI
4. Add Grafana; build one panel live
5. Roll the same helper to the other services
6. Add business counters and a second dashboard row
7. Stop before traces — call out Phase 2 explicitly

## Where the code lives

| Piece | Location |
|-------|----------|
| Shared OTel wiring | `Company.Shared/Extensions/TelemetryExtensions.cs` (`AddTelemetry`, `MapMetrics`) |
| Service name | Owned by each app (e.g. `ProductMetrics.ServiceName`; gateway uses `"api-gateway"` in `Program.cs`) |
| Business counters | `Services/*Metrics.cs` (+ `I*Metrics`) in Product, Cart, Notification |
| Prometheus scrape config | `infra/prometheus/prometheus.yml` (`file_sd_configs`) |
| Prometheus scrape targets | `infra/prometheus/targets/*.json` (add a service here — do not edit `prometheus.yml`) |
| Grafana provisioning | `infra/grafana/provisioning/` |

HTTP ports used for scrape (see `launchSettings.json`): Gateway `5121`, Product `5148`, Cart `5152`, Notification `5212`.

**Adding a service to monitoring:** append an entry to `infra/prometheus/targets/services.json` (or add another `*.json` / `*.yml` in that folder). Prometheus reloads targets within `refresh_interval` (~15s) — no Prometheus restart and no `prometheus.yml` change. Filter series by the `service` label (job is `services`). Keep labels lean: `service` + `env` are enough for Grafana variables; avoid duplicate labels that are identical on every target.

## Production vs this sample

| Concern | This sample (Phase 1) | Typical production |
|---------|----------------------|--------------------|
| Export | Each app exposes `/metrics`; Prometheus scrapes | Apps push **OTLP** to an OpenTelemetry Collector; Collector exports to Prometheus/Mimir |
| `/metrics` exposure | Open on localhost (learning only) | Not public — network policy, mesh, or no scrape endpoint at all |
| Auth on scrape | None | mTLS / allow-lists / private scrape network |
| Cardinality | Few labels, no user ids | Same rule, plus recording rules / aggregation |
| Traces | Deferred (Phase 2) | Usually shipped with metrics from day one (Tempo/Jaeger) |
| Dashboards | Build in Grafana UI (or provision later) | SLO dashboards + alerts (Alertmanager / Grafana Alerting) |

Phase 1 keeps the pull model so you can `curl /metrics` and see Prometheus targets turn green. When you outgrow that, swap the Prometheus AspNetCore exporter for `AddOtlpExporter()` and point services at a Collector.

## Run order (after implementation)

```bash
docker compose up -d
dotnet run --project src/Company.ProductService
dotnet run --project src/Company.CartService
dotnet run --project src/NotificationService
dotnet run --project src/ApiGateway
```

Then open:

- Prometheus: http://localhost:9090
- Grafana: http://localhost:3000 (admin / admin)
- Example raw metrics: `http://localhost:5148/metrics` (ProductService HTTP)

## Out of scope (Phase 1)

- Distributed tracing / Tempo / Jaeger
- Log aggregation (Loki)
- OpenTelemetry Collector
- Alertmanager / paging
- Securing `/metrics`
- EF Core / database pool dashboards
- Replacing in-memory stores
