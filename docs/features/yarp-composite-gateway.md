# Feature: YARP Composite Gateway + CartService

## Goal

Demonstrate a realistic microservice layout where:

- **External clients** hit one URL (`ApiGateway`) over REST.
- **YARP** forwards HTTP traffic to backend services (no business logic in the gateway).
- **ProductService** exposes both **REST** (public catalog) and **gRPC** (internal service-to-service).
- **CartService** exposes **REST** to users and calls **ProductService over gRPC** to resolve live product names and
  prices.

Kafka / NotificationService stay unchanged.

## Target architecture

```
Client (REST + Authorization: Bearer {userId})
    │
    ▼
ApiGateway  ── fake JWT auth + YARP transform (X-User-Id) ──┬── /products/** ──► ProductService (REST)
                                                             └── /cart/**     ──► CartService (REST)
                                                                                      │
                                                                                      └── gRPC ──► ProductService

ProductService ── Kafka ──► NotificationService
```

## Design decisions

| Topic                  | Decision                                                                                                                      |
|------------------------|-------------------------------------------------------------------------------------------------------------------------------|
| Gateway role           | YARP reverse proxy. Validates fake JWT on cart routes; forwards `X-User-Id` to backends via request transform.                |
| Product CRUD (public)  | REST on `ProductService`, proxied by YARP. No auth required.                                                                 |
| Cart auth (gateway)    | `Authorization: Bearer {userId}` (fake JWT for learning). Invalid/missing token → `401` on `/cart/**`.                        |
| User identity (internal) | Gateway sets `X-User-Id` on the proxied request. Microservices read it via `Company.Shared` helpers — JWT is not forwarded. |
| gRPC use case          | `CartService` enriches cart lines with latest product data via `IProductServiceClient`.                                       |
| Cart identity          | One cart per authenticated user; lazy-created on first access. `userId` is not in route or response body.                     |
| Shared product package | `Company.Shared.ProductService` — gRPC proto/client + Kafka events.                                                           |
| Internal HTTP          | **App traffic (YARP, gRPC):** `https://localhost:7251` / `https://localhost:7152` (dev cert; gateway skips validation in Development). **Observability / local REST bypass:** `http://localhost:5148` / `http://localhost:5152` (loopback; Prometheus scrape) — not a second “public” API. |
| Shared product logic   | `ProductService` singleton used by gRPC handler and REST endpoints.                                                           |

## Implementation tasks

### Phase 1 — ProductService dual protocol

- [x] Extract in-memory product logic into `Services/ProductService.cs`.
- [x] Refactor `ProductServiceGrpc` to delegate to `ProductService`.
- [x] Add REST endpoints (one file per operation) mirroring existing CRUD routes under `/products`.
- [x] Add HTTP error mapping (`Extensions/HttpExtensions.cs`) compatible with previous gateway JSON shape.
- [x] Register `ProductService` in DI.

### Phase 2 — CartService

- [x] Add project `Company.CartService`.
- [x] In-memory `CartService` keyed by `userId` (one cart per user).
- [x] REST endpoints:
    - `GET /cart` — view cart with live prices (lazy-created if empty)
    - `POST /cart/items` — add/update line `{ productId, quantity }`
    - `DELETE /cart/items/{productId}` — remove line
    - `DELETE /cart` — clear cart
- [x] Resolve user from `X-User-Id` header (`Company.Shared`); return `401` if missing.
- [x] Register `IProductServiceClient` pointing at ProductService gRPC address.

### Phase 3 — ApiGateway (YARP)

- [x] Remove BFF endpoints and gRPC client dependency from gateway.
- [x] Configure `ReverseProxy` routes/clusters in `appsettings.json`.
- [x] `MapReverseProxy()` as the main entry point for `/products/**` and `/cart/**`.
- [x] Keep a small root endpoint describing routed services.
- [x] Fake JWT authentication handler (`Bearer {userId}`) + `AuthenticatedUser` policy on cart route.
- [x] YARP request transform: strip client `X-User-Id`, forward authenticated user id to backends.

### Phase 4 — Solution & docs

- [x] Add `Company.CartService` to solution.
- [x] Add `docs/features` folder to Solution Items.
- [x] Update `README.md` and `.http` sample requests.

## Run order

```bash
docker compose up -d
dotnet build
dotnet run --project src/Company.ProductService
dotnet run --project src/Company.CartService
dotnet run --project src/NotificationService
dotnet run --project src/ApiGateway
```

## Manual test flow

1. **Create products** via gateway:

   `POST https://localhost:7080/products`

2. **Add to cart** (user `1`):

   `POST https://localhost:7080/cart/items` with header `Authorization: Bearer 1` and body
   `{ "productId": 1, "quantity": 2 }`

3. **View cart**:

   `GET https://localhost:7080/cart` with header `Authorization: Bearer 1`

4. **Update product price** via gateway, then **GET cart** again — line totals reflect live gRPC price.

5. **Create product** — NotificationService logs Kafka event (unchanged).

## Future extensions (optional learning)

- Replace fake JWT handler with real `AddJwtBearer` (transform code stays unchanged)
- Active health checks on clusters
- Load balancing with multiple destinations
- Order checkout service consuming cart + publishing `OrderPlacedEvent`
