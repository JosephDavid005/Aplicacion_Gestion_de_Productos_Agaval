# Especificación 002: Funcionalidades Opcionales y Avanzadas

## 1. Contexto y Objetivos
Implementar las funcionalidades complementarias descritas en la sección 3.5 de la prueba técnica de Agaval para enriquecer el sistema empresarial:
1. **Historial de Movimientos de Inventario:** Auditoría transaccional de cada ajuste de stock (Entradas y Salidas) en la tabla `MovimientosInventario` con consulta de historial por producto.
2. **CRUD Completo de Categorías:** Gestión integral de categorías de productos (listar, crear, editar, alternar estado activo) en backend y frontend.
3. **Paginación y Ordenamiento Avanzado:** Control visual escalable de volumen de datos en el listado de productos mediante Angular Material (`MatPaginator` y `MatSort`).

---

## 2. Requisitos Funcionales (Sintaxis EARS)

### Módulo 1: Movimientos de Inventario (Kardex)
- **RF-14 (Evento):** CUANDO se procesa exitosamente un ajuste de stock (`AjustarStockCommand`), EL SISTEMA registra un nuevo registro en la tabla `MovimientosInventario` con `ProductoId`, `TipoMovimiento` ('ENTRADA' o 'SALIDA'), `Cantidad`, `Fecha` (UTC/servidor) y `Observacion` opcional en la misma transacción.
- **RF-15 (Evento):** CUANDO el operador consulta `GET /api/productos/{id}/movimientos`, EL SISTEMA devuelve la lista cronológica descendente de los movimientos registrados para dicho producto.
- **RF-16 (Evento):** CUANDO el operador abre el diálogo o vista de historial de movimientos de un producto en el frontend, EL SISTEMA presenta una tabla o línea temporal con el tipo (chip verde para entrada, rojo para salida), cantidad, fecha y observaciones.

### Módulo 2: CRUD Completo de Categorías
- **RF-17 (Evento):** CUANDO el operador invoca `POST /api/categorias`, EL SISTEMA valida que el nombre sea obligatorio (1-100 caracteres) y único, y almacena la categoría retornando `201 Created`.
- **RF-18 (Evento):** CUANDO el operador invoca `PUT /api/categorias/{id}`, EL SISTEMA actualiza el nombre y estado de la categoría; SI el `id` no existe, ENTONCES responde `404 Not Found`.
- **RF-19 (Evento):** CUANDO el operador invoca `DELETE /api/categorias/{id}`, SI la categoría tiene productos asociados, ENTONCES el sistema rechaza la eliminación con `400 Bad Request` preservando la integridad referencial.
- **RF-20 (Evento):** CUANDO el operador accede a la feature `/categorias` en el frontend, EL SISTEMA presenta tabla de gestión con acciones de alta, edición y cambio de estado, cargada de forma perezosa (`lazy loading`).

### Módulo 3: Paginación y Ordenamiento Avanzado
- **RF-21 (Evento):** CUANDO el operador interactúa con la tabla de productos, EL SISTEMA permite ordenar columnas (nombre, precio, stock, fecha) mediante `MatSort`.
- **RF-22 (Evento):** CUANDO la cantidad de productos supera el tamaño de página configurado (5, 10, 25), EL SISTEMA pagina la visualización mediante `MatPaginator`.

---

## 3. Requisitos No Funcionales y Restricciones
- Persistencia estricta en SQL Server 2022 con foreign keys y constraints `CK_Movimientos_Tipo CHECK (TipoMovimiento IN ('ENTRADA','SALIDA'))`.
- Clean Architecture y CQRS estricto con MediatR para Categorías y Movimientos.
- Frontend Angular v22 Zoneless, Signals reactivos, `ChangeDetectionStrategy.OnPush`, SCSS BEM estricto, cero `any`, cero `NgModules`, cero `::ng-deep`.
- Ejecución 100% contenerizada en Docker Compose.
