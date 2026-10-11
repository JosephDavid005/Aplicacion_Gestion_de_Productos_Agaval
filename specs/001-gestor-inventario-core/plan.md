# Plan Técnico — Spec 001: Gestor de Inventario Core
**Basado en:** specs/001-gestor-inventario-core/spec.md (Estado: aprobada)

## 1. Arquitectura y Módulos Afectados
Monorepo: `backend/` (solución .NET 10) y `frontend/` (Angular v20+), más `database/`, `docker-compose.yml`, `docs/`.

**Backend (`backend/src`)** — dependencias: API→Application→Domain; Infrastructure→Application→Domain
- `GestorInventario.Domain`: `Entities/Producto`, `Entities/Categoria`, `Exceptions/DomainValidationException`. Producto valida invariantes (Nombre, Precio>0, Stock>=0), `IsLowStock`, `AdjustStock(tipo, cantidad)`.
- `GestorInventario.Application`: `Productos/Commands/{Crear,Editar,Eliminar,AjustarStock}`, `Productos/Queries/{Listar,ObtenerPorId,ListarStockBajo}`, handlers, DTOs, `Abstractions/{IProductoRepository,ICategoriaRepository,IUnitOfWork}`, pipeline behavior de validación (FluentValidation descartado, ver decisiones).
- `GestorInventario.Infrastructure`: `Persistence/PersistenceContext` (EF Core SqlServer), configuraciones de entidad, `Repositories/*`, `Seeds/DataSeeder`, `DependencyInjection.cs`.
- `GestorInventario.Api`: `Controllers/ProductosController`, `Program.cs`, middleware de excepciones→ProblemDetails (400/404), Swagger.
- Tests: `backend/tests/{Domain,Application}.Tests` (xUnit + FluentAssertions + NSubstitute) y `Api.IntegrationTests` (WebApplicationFactory, SQLite/InMemory solo para tests).

**Endpoints:** `POST/GET /api/productos`, `GET/PUT/DELETE /api/productos/{id}`, `GET /api/productos/stock-bajo`, `PATCH /api/productos/{id}/stock` `{tipo: ENTRADA|SALIDA, cantidad}`, `GET /api/categorias` (solo lectura, para el selector).

**Frontend (`frontend/src/app`)**: `core/{interceptors(error),services}`, `shared/components`, `features/productos/{components(producto-list, producto-form, producto-filters),services/productos.service.ts,models,routes.ts}`; `app.config.ts` con `provideZonelessChangeDetection`, `provideRouter`, `provideHttpClient(withInterceptors)`.

## 1.1 Topología de ejecución (Docker-only)
`docker-compose.yml` con servicios: `sqlserver` (1433, volumen, healthcheck), `api` (imagen `dotnet/sdk:10.0`, `dotnet watch run`, bind mount, 5000, depende de sqlserver healthy), `frontend` (imagen `node` LTS, `ng serve --host 0.0.0.0`, bind mount, 4200), `api-tests` y `frontend-tests` (profile `test`; el front con Chromium headless en imagen que lo incluya). Config y SA password en `.env` (+ `.env.example`). Dockerfiles multi-stage de producción: opcional al final.

## 2. Modelos, Tipos y Lógica de Dominio
- `Producto { Id, Nombre(<=150), Descripcion?(<=500), Precio decimal(10,2), Stock, StockMinimo(def 5), CategoriaId, FechaCreacion }`; `Categoria { Id, Nombre(<=100), Activo }`.
- Lógica pura en el dominio: validaciones, `IsLowStock => Stock < StockMinimo`, `AdjustStock`.
- Front: interfaces `Producto`, `Categoria`, `CrearProductoRequest`, `AjusteStockRequest`; `ProductosStore` con signals (`productos`, `filtros`, `productosFiltrados` computed).

## 3. Decisiones Técnicas y Alternativas Descartadas
- **MediatR 12.x** para CQRS (aprobado). *Descartado:* Wolverine (sobreingeniería, menor familiaridad); MediatR ≥13 (licencia).
- **Validación en Domain + behavior propio** en Application. *Descartado:* FluentValidation (dependencia extra sin necesidad; constitución exige minimalismo).
- **EF Core con migración + `database/script.sql`** equivalente al del PDF. *Descartado:* solo script manual (menos reproducible).
- **Borrado físico** (la tabla no tiene `Activo`).
- **Todo en Docker Compose** (SQL Server, API, frontend, tests). Tests de integración contra SQLite in-memory dentro del contenedor de tests. *Descartado:* ejecutar API/Angular en host.
- **Filtros en el cliente** (signals/computed) sobre el listado, dado el alcance sin paginación. *Descartado:* filtros server-side (innecesario ahora).
- **Tests frontend:** Jest-free; se usa el runner por defecto de Angular CLI. [Dependencias nuevas adicionales: preguntar antes].

## 4. Estrategia de Testing (Matriz de Cobertura)
| RF | Suite | Tipo |
| :--- | :--- | :--- |
| RF-1, RF-8 | `Infrastructure` / `Api.IntegrationTests` (seed + mapeo) | Integración |
| RF-2, RF-3 | `Domain.Tests/ProductoTests`, `Application.Tests/CrearProductoHandlerTests` | Unitario + error |
| RF-4, RF-5 | `Application.Tests/*Handler*Tests`, `Api.IntegrationTests/ProductosEndpointsTests` | Unitario + integración |
| RF-6 | `Application.Tests/ListarStockBajoHandlerTests` | Unitario |
| RF-7 | `Domain.Tests/ProductoTests` (AdjustStock), `Application.Tests/AjustarStockHandlerTests` | Unitario + error |
| RF-9 | Revisión de dependencias entre proyectos (test de arquitectura simple por reflexión) | Arquitectura |
| RF-10..13 | `*.spec.ts` por componente/servicio/store | Unitario Angular |

Comandos: `docker compose run --rm api-tests` y `docker compose run --rm frontend-tests`.

