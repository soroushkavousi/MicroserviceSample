# MicroserviceSample

A focused .NET sample that shows how common microservice pieces fit together in one runnable solution. It stays small on purpose so you can read the code end to end and try changes without a large production setup.

## Why this sample

- **One entry point** — clients call a single gateway URL instead of every service directly.
- **Unified API docs** — each service publishes its own OpenAPI document; the gateway hosts a single Scalar UI over them.
- **Mixed protocols** — REST for external APIs, gRPC for fast internal calls.
- **Async messaging** — Kafka events decouple services for side effects like notifications.
- **Shared building blocks** — consistent error handling, pagination, and identity headers across services.

## What you can learn

- **YARP** — route HTTP traffic from an API gateway to backend services
- **REST** — public HTTP APIs on each service, unified through the gateway
- **Scalar + OpenAPI** — interactive docs at the gateway that aggregate per-service OpenAPI specs
- **gRPC** — enrich cart data with live product prices from another service
- **Kafka + MassTransit** — publish and consume domain events
- **Result-based errors** — map domain failures to HTTP responses in a consistent way

## Architecture

```
Client (REST)
    │
    ▼
ApiGateway (YARP + auth)
    ├── /products/** ──► ProductService (REST)
    ├── /cart/**     ──► CartService (REST)
    │                       │  X-User-Id from gateway
    │                       └── gRPC ──► ProductService
    └── /docs (Scalar) ──► OpenAPI from ProductService & CartService

ProductService ── Kafka ──► NotificationService
```

Cart routes require `Authorization: Bearer {userId}`. The gateway validates the token and forwards the user id as `X-User-Id`. Backend services read that header — the JWT is not forwarded.

API reference is at **`/docs`** (Scalar, non-production). Each microservice owns its OpenAPI document; the gateway proxies those specs (`/openapi/products.json`, `/openapi/cart.json`) and renders them together.

## Services

| Project | Role |
|---------|------|
| `ApiGateway` | YARP reverse proxy for `/products/**` and `/cart/**`; Scalar UI at `/docs` |
| `Company.ProductService` | Product catalog — REST (public) + gRPC (internal); publishes `ProductCreatedEvent` |
| `Company.CartService` | Shopping cart REST API; reads live prices via gRPC |
| `NotificationService` | Kafka consumer — reacts to product-created events |
| `Company.Shared` | Common types: Result, pagination, errors, user identity, shared OpenAPI helpers |
| `Company.Shared.ProductService` | gRPC proto/client, Kafka events, ProductService Result types |

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for Kafka)

## Quick start

1. **Start Kafka** (Redpanda Console at http://localhost:8080):

   ```bash
   docker compose up -d
   ```

2. **Build and run** (separate terminals):

   ```bash
   dotnet build
   dotnet run --project src/Company.ProductService
   dotnet run --project src/Company.CartService
   dotnet run --project src/NotificationService
   dotnet run --project src/ApiGateway
   ```

3. **Try the API** — gateway base URL: `https://localhost:7080`

   **Suggested flow:**
   - Create a product: `POST /products`
   - NotificationService logs the Kafka event
   - Add to cart: `POST /cart/items` with `Authorization: Bearer 1`
   - View cart: `GET /cart` — prices come from ProductService via gRPC
   - Update a product price and refresh the cart to see live enrichment

   **Docs & samples:**
   - Scalar: [https://localhost:7080/docs](https://localhost:7080/docs) — Product and Cart documents; Bearer pre-filled in Development
   - HTTP file: [`src/ApiGateway/ApiGateway.http`](src/ApiGateway/ApiGateway.http)

## Further reading

- [YARP composite gateway design](docs/features/yarp-composite-gateway.md) — routing, auth, and service boundaries

## Scope

This is a teaching sample, not production-ready software. Persistence is in-memory, auth is simplified, and there is no deployment setup. Scalar and OpenAPI are non-production only. Use the sample to understand patterns and swap in real pieces (EF Core, JWT, health checks) as you grow the project.
