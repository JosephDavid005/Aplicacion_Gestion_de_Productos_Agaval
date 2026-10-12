# Plan de Arquitectura e Implementación — Spec 002

## 1. Diseño Backend (.NET 10 + Clean Architecture + CQRS)

### Entidades y Persistencia
- **Entidad `MovimientoInventario` (`Domain`):**
  - Propiedades: `Id`, `ProductoId`, `TipoMovimiento` ('ENTRADA' o 'SALIDA'), `Cantidad` (> 0), `Fecha` (DateTime UTC), `Observacion` (string? max 255).
  - Reglas de dominio: cantidad > 0, tipo válido, producto asociado.
- **Entidad `Categoria` (`Domain`):**
  - Ya existe, enriquecida con métodos de negocio para actualizar nombre y estado `Activo`.
- **EF Core (`Infrastructure`):**
  - Mapeo `MovimientoInventarioConfiguration` en `PersistenceContext` con constraint `CK_Movimientos_Tipo` y relación `FK_Movimientos_Productos`.
  - Migración / script de tabla `MovimientosInventario`.

### Casos de Uso (Application)
- **Movimientos:**
  - `AjustarStockCommand` actualizado para recibir `Observacion?: string` y guardar la auditoría transaccional de `MovimientoInventario`.
  - `ObtenerMovimientosPorProductoQuery` + `ObtenerMovimientosPorProductoQueryHandler` -> DTO `MovimientoInventarioDto`.
- **Categorías (CRUD completo):**
  - `CrearCategoriaCommand` / Handler.
  - `EditarCategoriaCommand` / Handler.
  - `EliminarCategoriaCommand` / Handler (valida que no tenga productos asociados).
  - `ListarCategoriasQuery` / Handler (ya existente o ampliado con flag para incluir inactivas en gestión).

### API Controllers
- `ProductosController`: nuevo endpoint `GET /api/productos/{id}/movimientos`.
- `CategoriasController`: endpoints `GET`, `GET {id}`, `POST`, `PUT {id}`, `DELETE {id}`.

---

## 2. Diseño Frontend (Angular v22 Zoneless + Signals + Material)

### Modelos y Servicios
- Tipos TypeScript: `MovimientoInventario`, `CrearCategoriaRequest`, `EditarCategoriaRequest`.
- `ProductosService`: método `getMovimientosByProductoId(id: number)`.
- `CategoriasService`: CRUD completo de categorías con `HttpClient`.

### Componentes y Vistas
- **Historial de Movimientos:**
  - Componente modal `MovimientosHistoricoDialogComponent` con tabla estilizada, chips distintivos (verde Entrada / rojo Salida) y observaciones.
  - En `ProductoListComponent`: botón de historial en la columna de acciones.
- **CRUD de Categorías (`features/categorias`):**
  - `CategoriaListComponent`: tabla Material con listado, badges de estado activo/inactivo, botón de crear, editar y eliminar.
  - `CategoriaFormDialogComponent`: modal reactivo tipado para crear o editar nombre y estado.
  - Lazy routes en `categorias.routes.ts` y enlace en el header principal (`app.component.html`).
- **Paginación y Ordenamiento:**
  - Integrar `MatPaginator` y `MatSort` en `ProductoListComponent`.

---

## 3. Plan de Pruebas TDD
- Pruebas unitarias de dominio y comandos MediatR.
- Pruebas de integración de API para endpoints de movimientos y CRUD de categorías.
- Pruebas unitarias en Vitest para stores, servicios y componentes nuevos.
- Validación de build y compilación en Docker.
