# AGENTS.md — Gestor de Inventario Agaval
Sistema full stack para la gestión de productos e inventario de retail para Agaval (.NET 10 + Angular v20+ + SQL Server).

## Stack y Tooling
- **Backend:** .NET 10 (C# 13), MediatR, EF Core, SQL Server (Docker / Local), xUnit / FluentAssertions para TDD.
- **Frontend:** Angular v20+ (Zoneless, Signals, OnPush, Standalone, Angular Material, SCSS BEM).
- **Tooling:** Docker + Docker Compose exclusivamente. Cero SDKs/runtimes en el host.
- **Comandos Clave (todos en contenedor):**
  - Levantar todo: `docker compose up -d`
  - Backend tests: `docker compose run --rm api-tests`
  - Frontend tests: `docker compose run --rm frontend-tests`
  - Comandos ad hoc: `docker compose run --rm api dotnet <cmd>` / `docker compose run --rm frontend ng <cmd>`

## Reglas Operativas
- Respetar `docs/constitution.md` y la spec activa en `specs/NNN-*/`.
- TDD obligatorio: test en rojo primero, luego implementación, luego verde.
- Ejecutar UNA sola tarea de `tasks.md` por ciclo de trabajo y parar.
- NUNCA ejecutar dotnet/node/bun/ng en el host; siempre vía Docker.
- Cumplimiento estricto del estándar de arquitectura de Agaval (cero `any`, cero `NgModules`, cero `::ng-deep`).

## Límites
- ✅ Siempre: Código y símbolos en inglés; specs, cuestionario y documentación en español.
- ⚠️ Preguntar antes: Instalar dependencias adicionales fuera del estándar o alterar el modelo de datos.
- 🚫 Nunca: Avanzar con tests en rojo, vibe-codear sin spec aprobada, escribir código fuera de los RFs.

## Memoria
- Al empezar: Leer `MEMORY.md` para conocer el estado y decisiones previas.
- Al terminar cada tarea: Actualizar `MEMORY.md` con el progreso y lecciones aprendidas.

