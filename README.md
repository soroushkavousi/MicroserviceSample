# MicroserviceSample

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/10.0)

MicroserviceSample is a practical .NET 10 solution designed to demonstrate microservice concepts and tools in a single, runnable setup. It consists of multiple independent projects acting as dedicated microservices.

External clients hit a unified API Gateway for REST requests, while internal communication between services is handled seamlessly through gRPC calls and Kafka event messaging.

## Architecture

```
Client (REST)
    │
    ▼
ApiGateway (YARP + auth)
    ├── /products/** ──► ProductService (REST)
    ├── /cart/**     ──► CartService (REST)
    │                       └── gRPC ──► ProductService
    └── /docs (Scalar)

ProductService ── Kafka ──► NotificationService
```

Stack: **YARP**, **gRPC**, **Kafka + MassTransit**, **OpenAPI / Scalar**, plus shared Result / pagination / identity helpers.

Cart calls need a Bearer token. The gateway validates it and forwards user identity to backends as a header — services never see the JWT.

| Project | Responsibility |
|---------|----------------|
| `ApiGateway` | Public entry point and Scalar docs UI |
| `Company.ProductService` | Product catalog (REST + gRPC) and product-created events |
| `Company.CartService` | Shopping cart API (uses ProductService for live prices) |
| `NotificationService` | Reacts to product-created events |
| `Company.Shared` | Shared helpers used by the services |
| `Company.Shared.ProductService` | Product gRPC contract/client and related event types |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
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

3. **Open the docs** at [https://localhost:7080/docs](https://localhost:7080/docs)

   Scalar is the easiest way to explore the Product and Cart APIs and send requests. In Development, Bearer auth is pre-filled for cart calls.

4. **Optional** — walk through a full flow with [`src/ApiGateway/ApiGateway.http`](src/ApiGateway/ApiGateway.http) (create a product → add to cart → view cart → change a price and refresh).

## Further reading

- [YARP composite gateway](docs/features/yarp-composite-gateway.md) — routing, auth, and how the services are split

## Scope

Learning / playground sample — not a production app. Auth is fake/simple and there's no deployment setup. In-memory storage keeps startup fast so the focus stays on the microservice wiring. When you outgrow that, swap in the real pieces (EF Core, JWT, health checks).
