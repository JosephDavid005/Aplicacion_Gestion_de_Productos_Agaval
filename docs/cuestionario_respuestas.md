# Cuestionario Técnico — Respuestas Conceptuales
**Candidato:** Joseph David Seguro Henao  
**Evaluación:** Desarrollador Full Stack — .NET 10 + Angular (v20+) | Gestor de Inventario Agaval  
**Fuente:** Sección 4 de `docs/referencia/Prueba_Tecnica.pdf`

---

## 4.1. Preguntas Conceptuales

---

### Backend

#### 1. ¿Por qué `ProductosController` no debe acceder directamente al `PersistenceContext` de EF Core, sino pasar siempre por los Handlers de `Application`? ¿Qué se gana con esa decisión frente a la alternativa, más rápida de escribir, de usar el `DbContext` directo en el controlador?

**Respuesta:**

Inyectar y consumir directamente el `PersistenceContext` (o `DbContext`) en el controlador viola dos principios fundamentales de arquitectura limpia y diseño SOLID:
1. **Principio de Responsabilidad Única (SRP):** El controlador debe limitarse a la gestión del protocolo de transporte HTTP (deserializar la petición, validar el modelo de entrada, invocar la operación y mapear el resultado al código de estado HTTP y formato de salida RFC 7807). No debe asumir responsabilidades de orquestación, reglas de negocio ni consultas a base de datos.
2. **Principio de Inversión de Dependencias (DIP):** Las capas de presentación exterior no deben acoplarse directamente a los detalles de infraestructura ni a las tecnologías de persistencia (EF Core / SQL Server).

**Beneficios concretos de mediar a través de los Handlers de `Application` (CQRS + MediatR):**
* **Testabilidad Aislada:** Los Handlers de Application se pueden probar unitariamente de forma instantánea sin necesidad de levantar el pipeline HTTP de ASP.NET Core, ni crear mocks de `HttpContext`, ni recurrir a bases de datos en memoria para probar lógica de negocio.
* **Reutilización y Multicanalidad:** El mismo caso de uso (`CrearProductoCommand`, `AjustarStockCommand`) puede ser invocado sin duplicar código desde un controlador REST, un consumidor de colas/eventos (RabbitMQ, Kafka, Azure Service Bus), un job en segundo plano (`IHostedService` / Hangfire) o una herramienta CLI.
* **Mantenibilidad y Evolución:** Si en el futuro se cambia el ORM (por ejemplo, introduciendo Dapper para lectura de alto rendimiento o migrando a otra base de datos), la capa de controladores y los contratos HTTP permanecen completamente intactos; el cambio queda confinado en la infraestructura.
* **Pipeline de Comportamientos Transversales:** Permite aplicar *Cross-Cutting Concerns* (validación automática vía FluentValidation, logging estructurado, auditoría, transacciones atómicas, métricas de rendimiento) de manera centralizada mediante `IPipelineBehavior` de MediatR sin ensuciar los controladores.

---

#### 2. Ante el requisito "al crear un producto con stock menor a su stock mínimo, se debe notificar por correo al área de compras", ¿por qué no debería implementar esa lógica directamente dentro de `CrearProductoCommandHandler`? ¿Qué alternativa de diseño elegiría y qué principio respalda esa decisión?

**Respuesta:**

**Por qué no implementarlo directamente en `CrearProductoCommandHandler`:**
* **Violación del Principio de Responsabilidad Única (SRP):** El propósito del comando `CrearProductoCommand` es garantizar las invariantes de negocio y persistir el nuevo producto en el repositorio. Enviar correos es una operación de notificación externa e infraestructura que no forma parte del núcleo transaccional del caso de uso.
* **Riesgo de Fallo y Bloqueo Transaccional:** El envío de un correo electrónico involucra I/O de red dependiente de un servicio externo (SMTP / SendGrid / AWS SES). Si el servidor de correos presenta latencia o está caído, la creación del producto en base de datos podría fallar, revertir la transacción o demorar la respuesta al usuario innecesariamente por un efecto secundario que no debería ser bloqueante.
* **Violación del Principio Open/Closed (OCP):** Si mañana el negocio solicita además enviar un mensaje a Slack, emitir un webhook y registrar una auditoría, tendríamos que modificar continuamente `CrearProductoCommandHandler`, aumentando el riesgo de regresiones.

**Alternativa de Diseño Elegida:**
Implementar el patrón **Domain Events / MediatR Notifications (`INotification`)** o el patrón **Outbox Transaccional**:
1. Tras guardar exitosamente el producto en base de datos, el handler o la entidad emite un evento: `ProductoCreadoEvent(int ProductoId, int Stock, int StockMinimo)`.
2. Un suscriptor desacoplado, `NotificarComprasPorStockBajoHandler : INotificationHandler<ProductoCreadoEvent>`, escucha el evento y se encarga exclusivamente de estructurar y despachar el correo (o encolarlo en un broker de mensajería asíncrono con reintentos).
3. **Principios que respaldan la decisión:**
   * **Open/Closed Principle (OCP):** Se pueden añadir nuevos suscriptores (Slack, SMS, métricas) sin tocar una sola línea del código de creación de productos.
   * **Single Responsibility Principle (SRP):** La persistencia del producto y la notificación viven en componentes separados e independientes.

---

### Frontend

#### 1. ¿Por qué el proyecto se construye 100% con componentes standalone en lugar de usar `NgModules`, como se hacía tradicionalmente en Angular? ¿Qué ventaja concreta le da esta decisión al lazy loading de features?

**Respuesta:**

A partir de Angular v14+ y consolidado en v17-v20+, los componentes **Standalone** representan el estándar moderno oficial del framework, eliminando la capa de abstracción intermedia de los `NgModules`.

**Ventajas concretas frente a NgModules tradicionales:**
* **Grafo de Dependencias Explícito y Local:** Cada componente declara exactamente lo que necesita en su propio arreglo `imports: [MatTableModule, CurrencyPipe, MatButtonModule]`. Se elimina el antipatrón de "módulos cajón de sastre" (`SharedModule`) donde se importaban decenas de librerías en lote para que estuvieran disponibles en toda la app.
* **Tree-Shaking Real y Reducción del Bundle:** Con `NgModules`, los bundlers (esbuild / Vite) tenían dificultades para determinar qué componentes exportados en un módulo se utilizaban realmente en tiempo de ejecución, provocando que código no utilizado terminara en los chunks de producción. Con Standalone, el árbol de dependencias es 100% estático y granular, permitiendo un tree-shaking óptimo que reduce radicalmente el peso transferido (en nuestro proyecto, el chunk inicial comprimido es de apenas ~96 kB).
* **Lazy Loading Directo y Simplificado:**
  * Con `NgModules`: Se requería declarar un módulo de rutas, un módulo de feature con `declarations` y `exports`, y cargarlo con `loadChildren: () => import('./feature.module').then(m => m.FeatureModule)`.
  * Con Standalone: Se enruta directamente al componente o a un archivo de rutas plano mediante `loadComponent: () => import('./producto-list.component').then(c => c.ProductoListComponent)` o `loadChildren: () => import('./productos.routes').then(r => r.PRODUCTOS_ROUTES)`. El código diferido solo se descarga cuando el usuario navega a la ruta, sin sobrecarga de módulos intermedios.
* **Menor Carga Cognitiva:** Se elimina el error clásico de desarrollador de olvidar declarar un componente en un módulo o importar módulos en el orden incorrecto.

---

#### 2. ¿Por qué se eligió `ChangeDetectionStrategy.OnPush` en lugar de la estrategia por defecto para los componentes del proyecto? ¿Qué implicaciones tiene esa decisión sobre la forma en que se deben actualizar los datos en el componente?

**Respuesta:**

**Por qué se eligió `OnPush`:**
* **Estrategia por defecto (`Default`):** Cada vez que ocurre un evento asíncrono en cualquier parte de la aplicación (un clic de mouse, una respuesta HTTP, un `setTimeout`, etc.), Angular recorre y verifica recursivamente **todo el árbol de componentes de la aplicación** de arriba a abajo buscando cambios. En aplicaciones empresariales con tablas de datos grandes, esto genera miles de comparaciones innecesarias por segundo, degradando el rendimiento visual y los cuadros por segundo (FPS).
* **Estrategia `OnPush`:** Le indica al motor de Angular que ignore el componente durante los ciclos generales de detección de cambios, a menos que ocurra uno de estos eventos específicos:
  1. La referencia de un `@Input()` cambie (por comparación estricta de referencia `===`).
  2. Un evento del DOM se origine dentro del propio componente o de uno de sus hijos.
  3. Se actualice un `Signal` reactivo consumido en la plantilla del componente.
  4. Se solicite explícitamente mediante `ChangeDetectorRef.markForCheck()`.

**Implicaciones sobre la forma de actualizar los datos en el componente:**
1. **Obligatoriedad de Inmutabilidad:** No se deben mutar objetos o arreglos existentes (ej. `this.productos.push(nuevo)` o `producto.stock = 10` no disparará la detección). Es mandatorio emitir una nueva referencia inmutable (ej. `this.productos.set([...this.productos(), nuevo])`).
2. **Sinergia Natural con Signals:** Los `Signals` de Angular (`signal()`, `computed()`) notifican de forma precisa al grafo reactivo de Angular qué nodos del DOM dependen de ese valor. Al combinar `OnPush` + `Signals` + Zoneless (`provideZonelessChangeDetection()`), Angular solo re-renderiza quirúrgicamente los elementos afectados sin evaluar ramas enteras del DOM.
3. **Código Predecible y Libre de Bugs:** Obliga al equipo a mantener un flujo de datos unidireccional y predecible, evitando efectos secundarios ocultos en bindings de plantillas.

---

### CI/CD, Docker y Kubernetes

#### 1. ¿Por qué definir los recursos de Kubernetes (Deployment, Service, ConfigMap, etc.) en archivos YAML versionados en el repositorio en lugar de crearlos manualmente con `kubectl create` desde la línea de comandos?

**Respuesta:**

Definir los manifiestos en archivos YAML versionados en Git implementa el paradigma de **Infraestructura como Código (IaC)** y la filosofía **GitOps**, contrastando con la administración imperativa manual ("ClickOps" o comandos aislados):

* **Trazabilidad y Auditoría (Audit Trail):** Cualquier modificación en réplicas, límites de memoria (`resources.limits`), variables de entorno o sondas de salud (`livenessProbe`/`readinessProbe`) queda registrada en los commits de Git con autor, fecha, mensaje y aprobación vía Pull Request.
* **Reproducibilidad y Recuperación ante Desastres (Disaster Recovery):** Si un clúster entero se destruye o se requiere crear un entorno nuevo idéntico (staging, QA o multi-región), basta con ejecutar `kubectl apply -f k8s/` o sincronizar con herramientas como ArgoCD / Flux para reconstruir la infraestructura en minutos de forma idéntica.
* **Prevención de Configuración Discrepante (*Configuration Drift*):** Los comandos imperativos (`kubectl scale`, `kubectl set env`) dejan al clúster en un estado volátil desconocido por el equipo que no sobrevive a reinicios o migraciones. Con YAMLs versionados, Git es la única fuente de la verdad (*Single Source of Truth*).
* **Validación Temprana y Linting en Pipelines:** Permite correr validadores automáticos en CI (como `kubeval`, `kube-linter` o escaneos de seguridad de Trivy/Checkov) para detectar malas prácticas (ej. contenedores corriendo como root, falta de límites de CPU) antes de que lleguen a producción.

---

#### 2. ¿Por qué empaquetar la aplicación en una imagen Docker en lugar de desplegar directamente el artefacto compilado sobre un servidor o VM? ¿Qué problema resuelve concretamente containerizar la app?

**Respuesta:**

**El problema concreto que resuelve:**
Resuelve el histórico síndrome de *"en mi máquina funciona, pero en el servidor falla"*. Cuando se despliega un binario compilado directo en un servidor o máquina virtual, el software depende del estado implícito del host: versiones de paquetes del sistema operativo, parches de seguridad, variables de entorno globales, librerías compartidas (`libc`, `OpenSSL`), runtimes preinstalados (.NET SDK/Runtime, Node/Bun) y puertos en conflicto.

**Beneficios de empaquetar en una imagen Docker:**
1. **Artefacto Inmutable y Hermético:** La imagen Docker empaqueta no solo el código compilado, sino todo su entorno de ejecución: el sistema operativo base mínimo (ej. Alpine / Debian distroless), el runtime exacto (.NET 10, Bun), dependencias del sistema y configuración. Lo que se probó y certificó en desarrollo es bit a bit lo mismo que corre en producción.
2. **Aislamiento Total de Dependencias:** Permite ejecutar múltiples aplicaciones o microservicios con versiones de .NET o Node incompatibles en el mismo servidor sin colisiones entre sí.
3. **Portabilidad Absoluta:** La imagen se ejecuta de forma idéntica en la laptop del desarrollador (Windows con WSL2, macOS, Linux), en servidores locales on-premise de Agaval o en cualquier nube pública (AWS ECS/EKS, Azure AKS, Google Cloud Run).
4. **Eficiencia y Efimeridad:** Los contenedores inician en segundos frente a los minutos que tarda levantar o aprovisionar una máquina virtual completa, facilitando el escalado horizontal elástico ante picos de demanda retail (ej. Cyberlunes, Black Friday).

---

#### 3. ¿Por qué separar el pipeline en stages independientes de Build y Release/Deploy en lugar de tener un único job que compile y despliegue todo de una vez? ¿Qué riesgo mitiga esa separación?

**Respuesta:**

Separar las fases de **Build** (Integración Continua - CI) y **Release/Deploy** (Entrega/Despliegue Continuo - CD) es un principio cardinal de DevOps y entrega continua (*Continuous Delivery*):

**Riesgos críticos que mitiga:**
1. **Riesgo de Despliegue de Código Roto (Fail-Fast):** Si las pruebas unitarias, el análisis estático de código (SonarQube) o la compilación fallan, el stage de Build aborta el pipeline inmediatamente. Se garantiza que **ningún artefacto defectuoso toque jamás la infraestructura de despliegue**.
2. **Violación del Principio "Build Once, Deploy Anywhere" (Consistencia de Artefactos):** Si se compila en un único job conjunto por entorno, se corre el grave riesgo de compilar el código en Staging y luego *volver a compilar* para Producción. Pequeñas diferencias temporales (actualización de una dependencia transitiva, diferencias de paquetes en el agente de CI) pueden provocar que a producción llegue un binario distinto al validado en QA. Al separarlo, el stage de Build genera una única imagen inmutable con su tag SHA, y los stages posteriores de Deploy se limitan a desplegar esa misma imagen exacta en los distintos ambientes.
3. **Seguridad y Principio de Menor Privilegio (Least Privilege):** El entorno de compilación (Build) necesita herramientas pesadas (compiladores, SDKs, acceso a internet para descargar paquetes de NuGet o npm), pero **no debe tener credenciales de administración del clúster de producción**. Por el contrario, el agente de Deploy solo requiere acceso a la API de Kubernetes o cloud provider, pero no necesita herramientas de compilación. Separar los stages evita la filtración de secretos de producción en caso de que una librería externa maliciosa se ejecute durante el build.
4. **Control de Despliegues y Aprobaciones Manuales:** Permite introducir compuertas de calidad y aprobaciones manuales de negocio (*Manual Gates*) entre el paso de Staging a Producción sin tener que recomenzar la compilación desde cero.
