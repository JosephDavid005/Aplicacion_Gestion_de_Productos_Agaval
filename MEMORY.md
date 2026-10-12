# MEMORY.md - Estado del Proyecto
Memoria viva entre sesiones. Mantener siempre en menos de 50 líneas.

## Estado Actual
- Versión / Hito: v1.2.0-spec-002-completada
- Specs Activas: 
  - `specs/001-gestion-productos-core/`: [10/10 tareas completadas (100%)]
  - `specs/002-opcionales-avanzados/`: [10/10 tareas completadas (100%)]
- Estado de Entrega: PROYECTO 100% BLINDADO Y LISTO PARA SUSTENTACIÓN TÉCNICA.
- Tests: 147/147 en verde (83 backend .NET 10 + 64 frontend Angular v22).
- Documentación: README.md ejecutivo con diagramas C4 en Mermaid, matriz de heurísticas de Nielsen, script.sql con seeds y despliegue en 1 comando vía Docker Compose. Bundle prod en Bun SPA (puerto 4200).

## Decisiones Importantes de Arquitectura (y por qué)
- Backend: Clean Architecture (.NET 10, C# 13) + CQRS (MediatR 12.x).
- Frontend: Angular v22 Zoneless, OnPush, Signals reactivos, SCSS BEM, Angular Material.
- MovimientoInventario (Kardex): Auditoría transaccional backend-frontend con modal histórico cronológico y badges ENTRADA / SALIDA.
- Categorías: API RESTful CRUD completa, middleware ProblemDetails para integridad referencial, UI modular con lazy loading (`CATEGORIAS_ROUTES`).
- UX Avanzada: Navbar con navegación activa y tabla con `MatSort` + `MatPaginator`.
- REGLA: 100% de ejecución en contenedores Docker (build, tests, scripts, runtime).

## Progreso por Tareas (Spec 002 - 100% DONE)
- T1: Entidad `MovimientoInventario`, mapeo en EF Core con constraint `CK_Movimientos_Tipo`, repositorio y auditoría en `AjustarStockCommandHandler`.
- T2: Query `ObtenerMovimientosPorProductoQuery`, DTO, y endpoint `GET /api/productos/{id}/movimientos`.
- T3: `MovimientosHistoricoDialogComponent` con tabla, badges de color y botón de historial en la tabla de productos.
- T4: Commands `CrearCategoriaCommand`, `EditarCategoriaCommand`, `EliminarCategoriaCommand` en Application.
- T5: Endpoints REST en `CategoriasController` con middleware para `InvalidOperationException` y tests de integración.
- T6: `CategoriasService` y `CategoriasStore` en Angular con Signals reactivos.
- T7: Componentes de UI de Categorías (`CategoriaListComponent`, `CategoriaFormDialogComponent`, `CATEGORIAS_ROUTES`).
- T8: Navbar global con navegación fluida y lazy routes (`/productos`, `/categorias`).
- T9: Paginación y ordenamiento con `MatPaginator` y `MatSort` integrados sobre `MatTableDataSource` en `ProductoListComponent`.
- T10: `database/script.sql` actualizado con tabla `MovimientosInventario` y seeds; `README.md` actualizado; suite 147/147 tests en verde.
