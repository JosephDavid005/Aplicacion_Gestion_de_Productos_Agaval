# MEMORY.md — Estado del Proyecto
Memoria viva entre sesiones. Mantener siempre en menos de 50 líneas.

## Estado Actual
- Versión / Hito: v0.1.0-bootstrap
- Spec Activa: `specs/001-gestor-inventario-core/` (aprobada; plan y tasks pendientes de aprobación)
- Progreso de Tareas: [1 de 10 tareas completadas: T1 OK]

## Decisiones Importantes de Arquitectura (y por qué)
- Backend: Clean Architecture (Domain, Application, Infrastructure, API) con CQRS (MediatR).
- Frontend: Angular v20+ con `provideZonelessChangeDetection()`, Signals reactivos, `ChangeDetectionStrategy.OnPush`, standalone y SCSS con BEM.
- Base de datos: SQL Server en contenedor Docker Compose con tablas `Categorias` y `Productos` (y opcional `MovimientosInventario`).

- REGLA: todo corre en contenedores Docker (build, scaffold, tests, servidores). Nunca en el host. Se corrigió plan/tasks/AGENTS/constitución el 2026-10-10.

- T1: stack Compose OK (sqlserver healthy, api :5000, api-tests con `docker compose --profile test run --rm api-tests`). Scaffold webapi trae WeatherForecast; se elimina en T6.

## Aprendizajes y Trampas a Evitar
- Prohibido el uso de `any`, `NgModules`, `*ngIf`/`*ngFor`, `::ng-deep` y `!important`.
- Validar que .NET 10 SDK esté configurado o aprovisionado para el backend.

## Pendiente al final (Opcional PDF 3.5)
- CRUD Categoría, MovimientosInventario, autenticación, paginación avanzada, tests extra, despliegue cloud. Solo tras completar lo obligatorio.
- Ignorar info no relevante para programar (ej. fechas límite).
- Ruta ajuste stock aprobada: PATCH /api/productos/{id}/stock {tipo,cantidad}.

## Próximos Pasos Inmediatos
- Aprobar plan.md y tasks.md (Compuerta 2). Decisión: MediatR 12.x aprobado.
- Ejecutar T2 (Dominio) tras revisión de T1.





