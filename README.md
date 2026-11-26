# BiteFlow — Guía del proyecto y división del trabajo

## 1. ¿De qué trata BiteFlow?

BiteFlow es un sistema web para apoyar la gestión de un restaurante. Permite mostrar la carta de productos, armar pedidos desde un carrito y llevar el control de las operaciones principales del negocio, como productos, categorías, usuarios, pagos y reportes.

La idea es que cada tipo de usuario pueda realizar las tareas que le corresponden. El cliente consulta la carta y registra sus pedidos; el personal de caja registra los pagos; y el administrador gestiona la información y revisa los resultados del negocio.

## 2. ¿Qué incluye el proyecto?

- **Carta y productos:** muestra los productos disponibles para que el cliente pueda revisar qué ofrece el restaurante.
- **Categorías:** organiza los productos por grupos para que la carta sea más fácil de consultar.
- **Carrito:** permite agregar productos, cambiar cantidades, quitar productos y revisar el importe antes de confirmar.
- **Pedidos:** registra los pedidos del cliente, permite consultar su historial y ver el detalle de cada pedido.
- **Pagos:** muestra los pedidos pendientes y permite registrar el pago desde el perfil autorizado.
- **Usuarios y acceso:** contempla el inicio de sesión, el registro de clientes y el acceso según el rol del usuario.
- **Reportes:** permite consultar información de ventas por fechas y revisar los resultados en páginas.
- **Administración:** reúne las opciones que utiliza el administrador para gestionar el sistema.

## 3. ¿Cómo funciona? (recorrido general)

1. El usuario ingresa a BiteFlow y consulta la carta.
2. Selecciona los productos que desea y los agrega al carrito.
3. Revisa las cantidades y confirma el pedido.
4. El pedido queda registrado para su atención y pago.
5. El cajero o administrador autorizado consulta los pedidos pendientes y registra el pago.
6. El administrador puede mantener los productos, categorías y usuarios, además de consultar los reportes de ventas.

El acceso a ciertas opciones depende del rol. Por eso, no todos los usuarios ven ni pueden realizar las mismas operaciones.

## 4. Perfiles de usuario

- **Cliente:** se registra o inicia sesión, consulta la carta, utiliza el carrito, confirma pedidos y revisa sus pedidos.
- **Cajero:** consulta los pedidos pendientes de pago y registra los pagos.
- **Administrador:** administra productos y categorías, revisa usuarios y consulta los reportes. También tiene acceso a las funciones de administración previstas en el sistema.

## 5. Organización del proyecto

El proyecto está organizado para separar las pantallas de las acciones que las controlan y de la información que se guarda:

- **Controllers:** reciben las acciones realizadas desde la web y coordinan lo que debe suceder. Aquí se encuentran los controladores de acceso, carrito, categorías, pedidos, pagos, productos, reportes y usuarios.
- **Views:** contienen las pantallas que ve el usuario.
- **Models:** representan la información que utiliza el sistema, como productos, categorías, pedidos, pagos y usuarios.
- **Data:** contiene la conexión y el acceso a la base de datos, además de los repositorios que apoyan las operaciones.
- **Data/Sp:** contiene los scripts SQL para preparar la base de datos y los procedimientos almacenados incluidos en el proyecto.

## 6. Herramientas utilizadas

- ASP.NET Core MVC con .NET 7.
- C# para la lógica del sistema.
- Razor para las vistas.
- SQL Server para almacenar la información.
- Entity Framework Core para varias operaciones de acceso a datos.
- ADO.NET para ejecutar los procedimientos almacenados utilizados por los módulos de pagos y reportes.
- Visual Studio 2022 como entorno de desarrollo.

## 7. ¿Cómo preparar y ejecutar el proyecto?

1. Descargar o clonar el repositorio.
2. Abrir `BiteFlow.sln` con Visual Studio 2022.
3. Restaurar los paquetes del proyecto.
4. Configurar `DefaultConnection` en `BiteFlow.Web/appsettings.json` para que apunte a la instancia de SQL Server que se utilizará.
5. Preparar la base de datos ejecutando, en este orden, los scripts de `BiteFlow.Web/Data/Sp/`:
   - `01_schema.sql`
   - `sp_registrar_pago.sql`
   - `sp_reporte_ventas_paginado.sql`
   - `02_seed.sql`
6. Ejecutar el proyecto `BiteFlow.Web` desde Visual Studio.

**Importante sobre los usuarios:** el script `02_seed.sql` no crea usuarios de prueba. Se puede registrar un cliente desde la web. Para probar los perfiles Administrador y Cajero, deben crearse los usuarios correspondientes en SQL con una contraseña almacenada en el formato compatible con `PasswordHasher` del proyecto.

## 8. División del trabajo del equipo

La siguiente es una propuesta de división por módulos. El equipo debe colocar los nombres de los responsables y confirmar la asignación antes de empezar. Cada integrante debe trabajar en su parte, probarla y subir sus propios cambios al repositorio.

### Integrante 1 — Productos y categorías

**Objetivo:** mantener la información que aparece en la carta del restaurante.

**Qué debe revisar o completar:**
- Listar los productos y sus categorías.
- Registrar productos con su nombre, descripción, precio, imagen y disponibilidad.
- Crear y administrar categorías.
- Comprobar que los productos estén asociados a la categoría correcta.
- Probar que los cambios se reflejen en la carta.

**Carpetas principales:** `Controllers/ProductosController.cs`, `Controllers/CategoriasController.cs`, `Data/`, `Models/`, `Views/Productos/` y `Views/Categorias/`.

### Integrante 2 — Carrito y pedidos

**Objetivo:** revisar el proceso que sigue el cliente desde que elige productos hasta que confirma un pedido.

**Qué debe revisar o completar:**
- Agregar productos al carrito.
- Cambiar cantidades, quitar productos y vaciar el carrito.
- Verificar el cálculo del importe.
- Confirmar pedidos y guardar sus detalles.
- Consultar el historial y el detalle de los pedidos del cliente.
- Probar que un cliente solo pueda consultar sus propios pedidos.

**Carpetas principales:** `Controllers/CarritoController.cs`, `Controllers/PedidosController.cs`, `Data/`, `Models/`, `Views/Carrito/` y `Views/Pedidos/`.

### Integrante 3 — Pagos y comprobantes

**Objetivo:** revisar el registro de los pagos de los pedidos.

**Qué debe revisar o completar:**
- Mostrar los pedidos pendientes de pago.
- Registrar el método y el monto del pago.
- Verificar que el pago corresponda al pedido correcto.
- Revisar la respuesta del registro y el número de comprobante.
- Probar los casos de pago correcto y de datos no válidos.

**Carpetas principales:** `Controllers/PagosController.cs`, `Data/PagoRepository.cs`, `Data/Sp/sp_registrar_pago.sql`, `Models/` y `Views/Pagos/`.

### Integrante 4 — Usuarios, acceso y permisos

**Objetivo:** revisar el ingreso al sistema y que cada perfil tenga acceso únicamente a las opciones que le corresponden.

**Qué debe revisar o completar:**
- Probar el registro de clientes y el inicio y cierre de sesión.
- Revisar los perfiles Cliente, Cajero y Administrador.
- Verificar las restricciones de acceso a cada módulo.
- Revisar el listado y el estado de los usuarios desde administración.
- Probar accesos permitidos y denegados.

**Carpetas principales:** `Controllers/AccountController.cs`, `Controllers/UsuariosController.cs`, `Data/AuthRepository.cs`, `Data/UsuarioRepository.cs`, `Data/PasswordHasher.cs`, `Models/` y `Views/Account/` y `Views/Usuarios/`.

### Integrante 5 — Administración y reportes (si el equipo tiene cinco integrantes)

**Objetivo:** revisar el panel administrativo y la consulta de resultados de ventas.

**Qué debe revisar o completar:**
- Verificar el acceso al panel de administración.
- Consultar reportes por fecha.
- Revisar los datos mostrados y la paginación.
- Probar búsquedas con distintos rangos de fechas.

**Carpetas principales:** `Controllers/HomeController.cs`, `Controllers/ReportesController.cs`, `Data/PagoRepository.cs`, `Data/Sp/sp_reporte_ventas_paginado.sql`, `Models/` y `Views/Home/` y `Views/Reportes/`.

> Si el equipo tiene menos integrantes, pueden combinar módulos relacionados. La asignación final debe acordarse entre todos para evitar que dos personas modifiquen la misma parte sin coordinarse.

## 9. Forma de trabajo y entrega de avances

1. Antes de modificar archivos, cada integrante debe actualizar su copia del repositorio.
2. Debe trabajar principalmente en el módulo que le fue asignado.
3. Antes de subir cambios, debe ejecutar el proyecto y probar las funciones que modificó.
4. Debe guardar avances mediante commits con mensajes claros, por ejemplo: `Actualiza gestión de categorías` o `Corrige confirmación de pedidos`.
5. Debe comunicar al equipo qué archivos modificó y qué pruebas realizó.
6. Si necesita cambiar algo que afecta a otro módulo, debe coordinarlo con el responsable de esa parte.
7. Al integrar los cambios, el equipo debe ejecutar el proyecto completo y comprobar el recorrido desde la carta hasta el pedido y el pago.

## 10. ¿Qué debe entregar cada integrante?

Cada responsable debe dejar:

- Su módulo revisado y funcionando según lo solicitado.
- Los cambios guardados en el repositorio mediante uno o más commits propios.
- Una breve explicación de qué hizo.
- Una evidencia de las pruebas realizadas (capturas o demostración).
- Una lista de los problemas que encontró y, si quedaron pendientes, qué falta resolver.

## 11. Acuerdos del equipo

| Módulo | Responsable | Estado / observaciones |
|---|---|---|
| Productos y categorías | Por asignar | Por revisar |
| Carrito y pedidos | Por asignar | Por revisar |
| Pagos y comprobantes | Por asignar | Por revisar |
| Usuarios y permisos | Por asignar | Por revisar |
| Administración y reportes | Por asignar / combinar | Por revisar |

**Nota:** esta guía explica el propósito del sistema y propone cómo organizar el trabajo. No significa que todos los módulos estén terminados al mismo nivel: cada integrante debe revisar el código existente, comprobar su funcionamiento y completar lo que el equipo acuerde.
