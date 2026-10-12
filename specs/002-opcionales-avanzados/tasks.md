# Tareas de Implementación — Spec 002: Funcionalidades Opcionales y Avanzadas

> **Reglas innegociables:**
> 1. Tareas atómicas (máximo 20-30 min por tarea). Máximo 10 tareas por spec.
> 2. Cada tarea implementa TDD estricto: test en rojo primero, luego código, luego test en verde.
> 3. Al terminar cada tarea, marcar el checkbox y DETENER la ejecución para revisión.

- [x] **T1. Backend: Entidad `MovimientoInventario`, mapeo en EF Core y auditoría en `AjustarStockCommand`** (Cubre: RF-14)
  - Archivos: `Domain/Entities/MovimientoInventario.cs`, `Infrastructure/Persistence/*`, `Application/Productos/Commands/AjustarStock/*`, tests
  - Hecho cuando: test unitario de `AjustarStockCommandHandler` verifica creación del movimiento; test de persistencia en verde.

- [x] **T2. Backend: Query `ObtenerMovimientosPorProductoQuery` y endpoint en `ProductosController`** (Cubre: RF-15)
  - Archivos: `Application/Productos/Queries/ObtenerMovimientos/*`, `Api/Controllers/ProductosController.cs`, tests
  - Hecho cuando: endpoint `GET /api/productos/{id}/movimientos` devuelve lista ordenada cronológicamente en verde.

- [x] **T3. Frontend: Diálogo de historial de movimientos y botón en tabla de productos** (Cubre: RF-16)
  - Archivos: `movimientos-historico-dialog/*`, `producto-list.component.*`, tests
  - Hecho cuando: specs en Vitest del modal de historial y apertura desde el listado en verde.

- [x] **T4. Backend: Commands `CrearCategoria`, `EditarCategoria` y `EliminarCategoria` con handlers** (Cubre: RF-17, RF-18, RF-19)
  - Archivos: `Application/Categorias/Commands/*`, tests unitarios
  - Hecho cuando: handlers probados con éxito, validación de nombre único y rechazo si tiene productos vinculados.

- [x] **T5. Backend: `CategoriasController` con endpoints REST completos y tests de integración** (Cubre: RF-17, RF-18, RF-19)
  - Archivos: `Api/Controllers/CategoriasController.cs`, `Api.IntegrationTests/*`
  - Hecho cuando: tests de integración prueban POST (201), PUT (200), DELETE (204/400) en verde.

- [x] **T6. Frontend: `CategoriasService` y `CategoriasStore` con Signals reactivos** (Cubre: RF-20)
  - Archivos: `features/categorias/services/*`, `features/categorias/store/*`, tests
  - Hecho cuando: specs de servicio y store en Vitest en verde.

- [x] **T7. Frontend: Feature `categorias` con tabla de gestión y modal de crear/editar** (Cubre: RF-20)
  - Archivos: `features/categorias/components/*`, `categorias.routes.ts`, tests
  - Hecho cuando: specs de tabla y modal de categorías en verde con lazy loading.

- [x] **T8. Frontend: Navegación global entre Productos y Categorías en el Navbar** (Cubre: RF-20)
  - Archivos: `app.component.html`, `app.component.scss`, `app.routes.ts`, tests
  - Hecho cuando: tests de navegación en verde y rutas lazy integradas.

- [x] **T9. Frontend: Paginación y ordenamiento con `MatPaginator` y `MatSort` en la tabla de productos** (Cubre: RF-21, RF-22)
  - Archivos: `producto-list.component.*`, tests
  - Hecho cuando: specs en Vitest validan que `MatSort` y `MatPaginator` interactúan correctamente con el catálogo.

- [x] **T10. Actualización de `script.sql`, `README.md` y suite completa de pruebas full-stack** (Cubre: DoD Spec 002)
  - Archivos: `database/script.sql`, `README.md`, `MEMORY.md`
  - Hecho cuando: suites de backend y frontend pasan 100% en verde dentro de Docker y documentación actualizada.
