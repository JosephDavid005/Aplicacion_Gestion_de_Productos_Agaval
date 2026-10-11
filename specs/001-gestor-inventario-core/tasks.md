# Tareas de Implementación — Spec 001: Gestor de Inventario Core
> **Reglas innegociables:**
> 1. Tareas atómicas (máximo 20-30 min por tarea). Máximo 10 tareas por spec.
> 2. Cada tarea implementa TDD estricto: test en rojo primero, luego código, luego test en verde.
> 3. Al terminar cada tarea, marcar el checkbox y DETENER la ejecución para revisión.

- [x] **T1. Docker Compose (sqlserver, api, api-tests) + scaffold .NET vía contenedor + test de arquitectura** (Cubre: RF-9)
  - Archivos: `backend/*.sln`, 4 proyectos src + tests, `docker-compose.yml`, `.gitignore`
  - Hecho cuando: test de dependencias entre capas en rojo→verde con `docker compose run --rm api-tests`; `docker compose up -d` levanta SQL Server sano y la API.

- [x] **T2. Dominio: Producto y Categoria con validaciones y AdjustStock** (Cubre: RF-3, RF-6, RF-7)
  - Archivos: `Domain/*`, `Domain.Tests/*`
  - Hecho cuando: tests de límites (Precio<=0, Stock<0, nombre vacío, Stock==Minimo, salida>stock) en verde.

- [ ] **T3. Application: Commands Crear/Editar/Eliminar con handlers y puertos** (Cubre: RF-2, RF-3, RF-5, RF-9)
  - Archivos: `Application/Productos/Commands/*`, `Application.Tests/*`
  - Hecho cuando: handlers probados con repositorios simulados (éxito, inválido, no encontrado, categoría inexistente).

- [ ] **T4. Application: Queries Listar/ObtenerPorId/StockBajo + AjustarStock** (Cubre: RF-4, RF-6, RF-7)
  - Archivos: `Application/Productos/Queries/*`, `AjustarStockCommand`, tests
  - Hecho cuando: tests unitarios en verde, incluido 404 y salida inválida.

- [ ] **T5. Infrastructure: PersistenceContext, repositorios, migración, seed y script.sql** (Cubre: RF-1, RF-8)
  - Archivos: `Infrastructure/*`, `database/script.sql`
  - Hecho cuando: test de integración verifica mapeo, constraint de stock y seed (3 categorías + productos).

- [ ] **T6. API: ProductosController, categorías (lectura), manejo de errores, Swagger** (Cubre: RF-2..RF-9)
  - Archivos: `Api/*`, `Api.IntegrationTests/*`
  - Hecho cuando: tests de endpoints con códigos 201/400/404/200 en verde y Swagger accesible.

- [ ] **T7. Servicios frontend/frontend-tests en Compose + scaffold Angular (vía contenedor) zoneless + Material + core/shared/features + modelos y servicio HTTP** (Cubre: RF-13)
  - Archivos: `frontend/*`, `features/productos/{models,services}`, `app.config.ts`
  - Hecho cuando: spec del servicio (HttpTestingController) en verde; build sin `any`/NgModules.

- [ ] **T8. Store con Signals y listado en tabla con indicador de stock bajo** (Cubre: RF-10, RF-13)
  - Archivos: `features/productos/components/producto-list/*`, store, `routes.ts` (lazy)
  - Hecho cuando: specs del store y componente (OnPush, @for, BEM) en verde.

- [ ] **T9. Formulario reactivo tipado de crear/editar con selector de categoría** (Cubre: RF-11)
  - Archivos: `features/productos/components/producto-form/*`
  - Hecho cuando: specs de validación y envío en verde.

- [ ] **T10. Filtros por categoría/estado de stock, ajuste de stock en UI y README** (Cubre: RF-12, RF-7)
  - Archivos: `producto-filters/*`, diálogo de ajuste, `README.md`
  - Hecho cuando: specs de filtrado en verde; README permite ejecutar backend+frontend desde cero.



