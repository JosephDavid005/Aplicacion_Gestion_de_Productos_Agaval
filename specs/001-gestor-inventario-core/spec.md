# Spec 001 — Gestor de Inventario Core
**Estado:** aprobada
**Fuentes:** `docs/referencia/Prueba_Tecnica.pdf`, `docs/referencia/Estandar_Arquitectura.pdf`

## 1. Contexto y Objetivo
Prueba técnica Agaval (Auxiliar de Desarrollo): aplicación full stack para gestionar productos de una pequeña empresa, asociarlos a una categoría precargada y visibilizar productos con stock bajo, cumpliendo estrictamente el Estándar de Arquitectura (.NET 10 Clean Architecture + CQRS; Angular v20+ AGAVAL). Entrega: repo Git, script/migración SQL, README, respuestas al cuestionario.

## 2. Usuarios / Actores
- Operador de inventario (UI Angular).
- Evaluador técnico (ejecuta con README; evalúa arquitectura, estándar, funcionalidad, calidad y respuestas).

## 3. Historias de Usuario
- **HU-1:** Como operador, quiero crear, listar, ver, editar y eliminar productos para mantener el catálogo.
- **HU-2:** Como operador, quiero elegir la categoría de una lista precargada al crear/editar.
- **HU-3:** Como operador, quiero ver y filtrar productos con stock bajo para reponerlos.
- **HU-4:** Como operador, quiero ajustar stock (entrada/salida) de un producto.

## 4. Requisitos Funcionales (EARS)
### Backend
- **RF-1 (Permanente):** EL SISTEMA persiste en SQL Server (`GestorInventarioDB`) las tablas `Categorias` y `Productos` con los tipos, defaults y constraints del script de la prueba (incl. `CK_Productos_Stock`).
- **RF-2 (Evento):** CUANDO se crea un producto válido con una `CategoriaId` existente, EL SISTEMA lo persiste y responde 201 Created con el recurso.
- **RF-3 (Error):** SI Nombre está vacío, Precio <= 0, Stock < 0 o la `CategoriaId` no existe, ENTONCES EL SISTEMA responde 400 con detalle por campo y no persiste.
- **RF-4 (Evento):** CUANDO se consulta el listado u obtención por id, EL SISTEMA devuelve el producto (con su categoría); SI el id no existe, ENTONCES responde 404.
- **RF-5 (Evento):** CUANDO se edita o elimina un producto existente, EL SISTEMA aplica el cambio aplicando las mismas validaciones de RF-3; SI no existe, ENTONCES responde 404.
- **RF-6 (Evento):** CUANDO se invoca `GET /api/productos/stock-bajo`, EL SISTEMA devuelve solo productos con Stock < StockMinimo.
- **RF-7 (Evento):** CUANDO se registra un ajuste de stock (ENTRADA/SALIDA) con cantidad > 0, EL SISTEMA actualiza el Stock; SI la salida deja Stock < 0 o la cantidad <= 0, ENTONCES responde 400 sin modificar.
- **RF-8 (Permanente):** EL SISTEMA precarga categorías (Electrónica, Oficina, Aseo) y algunos productos de ejemplo, y documenta la API con Swagger.
- **RF-9 (Permanente):** EL SISTEMA separa Commands y Queries (CQRS) con Handlers en Application; los Controllers solo delegan por MediatR/Wolverine y nunca usan `PersistenceContext`.
### Frontend
- **RF-10 (Evento):** CUANDO el operador abre la feature `productos`, EL SISTEMA muestra tabla Angular Material con indicador visual para stock bajo.
- **RF-11 (Evento):** CUANDO el operador crea/edita, EL SISTEMA presenta formulario reactivo tipado con la categoría en una lista; SI es inválido, ENTONCES muestra errores y no envía.
- **RF-12 (Evento):** CUANDO el operador filtra por categoría y/o estado de stock (bajo/normal), EL SISTEMA actualiza el listado.
- **RF-13 (Permanente):** EL SISTEMA gestiona estado con Signals, componentes standalone OnPush, rutas de la feature con lazy loading, zoneless.

## 5. Requisitos No Funcionales
Estándar completo de `docs/constitution.md`; cero anti-patrones prohibidos; funcional al 100% al revisar; README con ejecución backend/frontend.

## 6. Casos Límite
Nombre solo espacios; Precio con >2 decimales o > DECIMAL(10,2); Descripción > 500 / Nombre > 150; Stock == StockMinimo (no es bajo); salida exacta al stock disponible (Stock=0 válido); eliminar producto inexistente.

## 7. Fuera de Alcance por ahora (opcional PDF 3.5 — se abordará AL FINAL, tras completar todo lo obligatorio)
CRUD de Categoría, `MovimientosInventario`, autenticación, paginación avanzada, despliegue en nube. Pruebas unitarias: se harán por TDD (constitución) aunque sean opcionales.

## 8. Criterios de Finalización (DoD)
- RF-1..13 verificados; tests en verde; cero `any`, NgModules, `!important`, `::ng-deep`.
- Entregables: repo, `database/script.sql` o migración, README, `docs/cuestionario.md` con las 7 respuestas (2 backend, 2 frontend, 3 CI/CD-Docker-K8s).

## 9. Dudas Abiertas
- [RESUELTO] Ajuste de stock: `PATCH /api/productos/{id}/stock` con `{tipo, cantidad}` (aprobado por Joseph).
- [NECESITA ACLARACIÓN] ¿Eliminación física o lógica? (la tabla Productos no tiene campo `Activo`; propongo física).
- [NECESITA ACLARACIÓN] ¿Incluimos `MovimientosInventario` como plus (registrar cada ajuste)? Propongo no, hasta terminar lo obligatorio.
- [NECESITA ACLARACIÓN] ¿MediatR o Wolverine? Propongo MediatR (dependencia nueva; requiere tu aprobación).


