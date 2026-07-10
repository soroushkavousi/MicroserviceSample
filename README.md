## MicroserviceSample

Hands-on .NET sandbox for exploring microservice patterns and tools. Small by design - for learning and portfolio use,
not production.

### What it covers

- **YARP** - HTTP reverse proxy gateway routing to backend services
- **gRPC** - internal service-to-service calls (CartService → ProductService)
- **REST** - public HTTP APIs on each service, unified entry via gateway
- **Kafka + MassTransit** - async messaging between services
- **Shared patterns** - Result-based error handling, pagination, domain events

### Services

| Project                         | Role                                                                                                |
|---------------------------------|-----------------------------------------------------------------------------------------------------|
| `Company.ProductService`        | Product catalog: **REST** (public) + **gRPC** (internal); publishes `ProductCreatedEvent` on create |
| `Company.CartService`           | Shopping cart **REST** API; reads live product prices via **gRPC**                                  |
| `ApiGateway`                    | **YARP** reverse proxy — forwards `/products/**` and `/cart/**`; fake JWT auth + `X-User-Id` header for cart |
| `NotificationService`           | Kafka consumer; reacts to product-created events                                                    |
| `Company.Shared`                | Common types (Result, pagination, errors, user identity headers)                                    |
| `Company.Shared.ProductService` | gRPC proto/client, Kafka events, shared Result types for ProductService                             |

### How it fits together

```
Client (REST)
    │
    ▼
ApiGateway (YARP + fake JWT auth)
    ├── /products/** ──► ProductService (REST, public)
    └── /cart/**     ──► CartService (REST, requires Bearer {userId})
                              │  X-User-Id header set by YARP transform
                              └── gRPC ──► ProductService

ProductService ── Kafka ──► NotificationService
```

See [docs/features/yarp-composite-gateway.md](docs/features/yarp-composite-gateway.md) for the full feature plan.

### Quick start

**Prerequisites:** .NET SDK, Docker

1. Start Kafka (includes Redpanda Console at http://localhost:8080):

   ```bash
   docker compose up -d
   ```

2. Build and run (each in a separate terminal):

   ```bash
   dotnet build
   dotnet run --project src/Company.ProductService
   dotnet run --project src/Company.CartService
   dotnet run --project src/NotificationService
   dotnet run --project src/ApiGateway
   ```

3. Create a product via the gateway REST API (`POST /products`). ProductService publishes the event and
   NotificationService logs the notification.

4. Add items to a user's cart (`POST /cart/items`) and view it (`GET /cart`). Send
   `Authorization: Bearer {userId}` via the gateway (fake JWT for learning). Each user has one lazy-created cart.
   Update a product price and refresh the cart to see live gRPC enrichment.
