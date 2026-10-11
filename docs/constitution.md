# Constitución del Proyecto — Gestor de Inventario Agaval
Principios innegociables basados en el Estándar de Arquitectura (.NET 10 + Angular v20+) de Agaval. Toda spec, plan técnico y tarea de implementación deben cumplirlos sin excepción.

1. **La Spec Manda (Spec Ante Omnia):** Ninguna línea de código de producción o refactor se escribe sin estar respaldada por un Requisito Funcional (RF) aprobado en `specs/NNN-*/spec.md` bajo sintaxis EARS.
2. **Arquitectura Backend Estricta (.NET 10 - Clean Architecture + CQRS):**
   - **Domain:** Entidades (`Producto`, `Categoria`) y reglas puras sin dependencias a frameworks ni capas externas.
   - **Application:** Casos de uso segregados en Commands y Queries (CQRS) vía MediatR/Wolverine, Handlers y Puertos (interfaces de repositorios).
   - **Infrastructure:** EF Core `PersistenceContext`, repositorios concretos mapeados a SQL Server y Seeds.
   - **API:** Controllers desacoplados que delegan a MediatR, Swagger y pipeline HTTP. La capa exterior depende de la interior; nunca a la inversa.
3. **Arquitectura Frontend Estricta (Angular v20+ - Estándar AGAVAL):**
   - **100% Standalone & Zoneless:** Uso obligatorio de `provideZonelessChangeDetection()`, `provideRouter()` y `provideHttpClient()`. Prohibido `zone.js` y `NgModules`.
   - **Reactividad:** `Signals` obligatorio para estado. Prohibido mezclar suscripciones manuales innecesarias.
   - **Rendimiento:** Todos los componentes con `ChangeDetectionStrategy.OnPush`. Carga diferida (lazy loading) para rutas y features.
   - **Control Flow:** `@if`, `@for`, `@switch` exclusivamente (prohibido `*ngIf`, `*ngFor`).
   - **Estructura:** `src/app/{core(authentication,guards,interceptors,services), shared(components,pipes,directives), features/<nombre>(components,services,models,routes.ts)}`; por componente: `.ts`, `.html`, `.scss`, `.spec.ts`. HTML semántico (nav, article, header).
   - **Anti-patrones prohibidos:** `any`, lógica de negocio o funciones pesadas en el template, estilos inline, componentes con múltiples responsabilidades, `!important`, `::ng-deep`, NgModules.
   - **Tipado & Estilos:** Cero `any`. Formularios reactivos estrictamente tipados. SCSS con metodología BEM estricta (`.bloque__elemento--modificador`). Angular Material temado sin `::ng-deep` ni `!important`.
4. **TDD como Puerta Obligatoria:** Primero test en rojo, luego código mínimo en verde, luego parada obligatoria para feedback.
5. **Criterios Verificables (Zero Vibe-Coding):** Validación exacta de reglas de negocio: `Precio > 0`, `Stock >= 0`, `Nombre` obligatorio, y condición `Stock < StockMinimo` para alerta de stock bajo.
6. **Convenciones y Lenguaje:** Código, variables, métodos y clases en inglés; documentación, specs SDD, cuestionario técnico y comunicación en español.
7. **Contenedores obligatorios (Docker-only):** NADA se ejecuta en la máquina local: ni build, ni scaffolding, ni tests, ni servidores de desarrollo. Todo corre vía Docker Compose (SQL Server, API .NET 10, Angular, runners de tests) con hot reload por bind mounts. Los comandos de scaffolding usan `docker run`/`docker compose run --rm` con imágenes oficiales (`mcr.microsoft.com/dotnet/sdk:10.0`, `node`). Prohibido depender de SDKs o runtimes instalados en el host.

8. **Fuente de verdad:** `docs/referencia/Prueba_Tecnica.pdf` y `docs/referencia/Estandar_Arquitectura.pdf`. Regla de dependencia: API->Application->Domain; Infrastructure->Application->Domain. Controllers delegan vía MediatR (o Wolverine); nunca acceden al `PersistenceContext`.


