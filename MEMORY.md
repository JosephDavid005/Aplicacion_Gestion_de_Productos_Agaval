# MEMORY.md - Estado del Proyecto
Memoria viva entre sesiones. Mantener siempre en menos de 50 líneas.

## Estado Actual
- Versión / Hito: v0.2.0-frontend-foundation
- Spec Activa: `specs/001-gestor-inventario-core/`
- Progreso de Tareas: [7 de 10 tareas completadas: T1..T7 OK]

## Decisiones Importantes de Arquitectura (y por qué)
- Backend: Clean Architecture (.NET 10) + CQRS (MediatR 12.x). 61/61 tests en verde.
- Frontend: Angular v22 Zoneless (`provideZonelessChangeDetection()`), OnPush, Signals reactivos, SCSS BEM, Angular Material.
- Runtime & Package Manager JS/TS: **Bun** (con soporte Node 22 en contenedor Docker para Vitest/JSDOM).
- Base de datos: SQL Server 2022 en Docker Compose con seeding inicial.
- REGLA: 100% de ejecución en contenedores Docker (build, tests, scripts, runtime).

## Progreso por Tareas
- T1..T6: Backend .NET 10 100% finalizado (61 tests unitarios e integrales en verde).
- T7: Frontend scaffold zoneless, Bun como runtime/gestor, Angular Material, Core/Shared/Features, modelos tipados, interceptor ProblemDetails RFC 7807 y ProductosService. 8 tests de frontend en verde (Vitest) y bundle prod OK.

## Próximos Pasos Inmediatos
- T8: Store con Signals (`productos.store.ts`) y listado en tabla Angular Material con indicador visual de stock bajo y OnPush.
