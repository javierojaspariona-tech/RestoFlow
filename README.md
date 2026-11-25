# BiteFlow

Sistema web de gestión de pedidos y ventas para restaurantes (pedidos para llevar).
Proyecto del curso **Desarrollo de Servicios Web I** — Computación e Informática, Cibertec.

> **Estado del tronco:** estructura, modelos, servicios base y autenticación inicial.
> **No se pudo compilar ni ejecutar al generarlo** (el entorno donde se creó no tenía el SDK de .NET ni acceso a NuGet).
> La primera persona que lo abra debe compilar (`Ctrl+Shift+B`) y avisar de cualquier error antes de que el resto empiece. Ver [Verificación pendiente](#verificación-pendiente).

## 1. Requisitos

| Herramienta | Versión |
|---|---|
| Visual Studio 2022 | 17.6 o superior, con la carga de trabajo **ASP.NET y desarrollo web** |
| .NET SDK | **7.0** (el mismo del proyecto del profesor). Si Visual Studio no lo trae: <https://dotnet.microsoft.com/download/dotnet/7.0> |
| SQL Server | **LocalDB** (`(localdb)\MSSQLLocalDB`, viene con Visual Studio) o SQL Server Express |
| Git | Cualquier versión reciente |

Paquetes NuGet (iguales al proyecto del profesor): `Microsoft.EntityFrameworkCore` 7.0.20, `...SqlServer` 7.0.20, `...Tools` 7.0.20, `Microsoft.VisualStudio.Web.CodeGeneration.Design` 7.0.12.

## 2. Cómo ejecutar el proyecto

1. Clona el repositorio y abre **`BiteFlow.sln`** en Visual Studio (se restauran los paquetes solos).
2. Crea la base de datos. En **Ver > Explorador de objetos de SQL Server > (localdb)\MSSQLLocalDB** (o en SSMS) ejecuta, en este orden:
   1. `Database/01_schema.sql` — crea la base `BiteFlowDb`, tablas, restricciones e índices.
   2. `Database/Sp/sp_registrar_pago.sql`
   3. `Database/Sp/sp_reporte_ventas_paginado.sql`
   4. `Database/02_seed.sql` — roles, usuarios de prueba, categorías y productos de ejemplo.
3. Presiona **F5**. Inicia sesión con un usuario de prueba (contraseña de los tres: `Biteflow123*`):

| Rol | Correo |
|---|---|
| Administrador | `admin@biteflow.local` |
| Cajero | `cajero@biteflow.local` |
| Cliente | `cliente@biteflow.local` |

**Usuarios de prueba solo para desarrollo:** cámbialos o elimínalos antes de la entrega final.

**¿Usas otra instancia de SQL Server?** No edites `appsettings.json`. Click derecho en el proyecto > **Administrar secretos de usuario** y agrega:

```json
{ "ConnectionStrings": { "DefaultConnection": "Server=.\\SQLEXPRESS;Database=BiteFlowDb;Trusted_Connection=True;TrustServerCertificate=True;" } }
```

## 3. Estructura

```
BiteFlow/
├── BiteFlow.sln
├── README.md
├── docs/GITHUB.md               Guía de GitHub para los 4 integrantes
├── .github/                     Plantilla de Pull Request
├── Database/
│   ├── 01_schema.sql            Esquema (fuente de verdad de la BD)
│   ├── 02_seed.sql              Datos iniciales
│   └── Sp/                      Procedimientos almacenados (uno por archivo)
└── BiteFlow.Web/
    ├── Program.cs               Configuración: EF Core, sesión, cookies, roles
    ├── Common/                  DomainException, ModuloControllerBase
    ├── Data/                    AppDbContext, Configurations/ (1 por entidad), PagoRepository (ADO.NET + SP)
    ├── Models/                  Entities/, Enums/, Dtos/, ViewModels/
    ├── Security/Roles.cs        Constantes de roles
    ├── Services/                Interfaces/ (contratos), Implementations/, ServiceRegistration.cs
    ├── Controllers/             HomeController
    ├── Areas/
    │   ├── Seguridad/           Javier
    │   ├── Catalogo/            Valeria
    │   ├── Pedidos/             Raúl
    │   └── Caja/                Montaño
    ├── Views/Shared/            _Layout (menú por rol), EnConstruccion, Error
    └── wwwroot/                 Bootstrap/jQuery (copiados de la maqueta del profesor)
```

Cada Area tiene su carpeta `Controllers/` y `Views/`. **Trabaja dentro de tu Area** y los conflictos de Git serán mínimos.

## 4. Modelo de datos

`Rol 1─N Usuario` · `Usuario 1─0..1 Cliente` · `Categoria 1─N Producto` · `Cliente 1─N Pedido` · `Pedido 1─N DetallePedido N─1 Producto` · `Pedido 1─0..1 Pago N─1 Usuario(cajero)`

| Tabla | Claves y restricciones importantes |
|---|---|
| Rol | `Nombre` único |
| Usuario | `Correo` único; `PasswordHash` (PBKDF2); `Activo` |
| Cliente | `Dni` único (8 dígitos); `IdUsuario` nulo y **único filtrado** (un usuario ↔ un solo cliente, sin duplicados) |
| Categoria / Producto | `Activo` (eliminación lógica) y `Disponible` (solo Producto). **Sin stock.** `Precio decimal(10,2)`. Producto único por (categoría, nombre) |
| Pedido | `Estado` (Pendiente, Pagado, Anulado); `Total decimal(10,2)`; `RowVersion` (concurrencia) |
| DetallePedido | `PrecioUnitario` = **precio histórico** al comprar; único por (pedido, producto) |
| Pago | `IdPedido` **único** (un pago por pedido); `MetodoPago`; `NumeroComprobante` único (INT-00000001) |

Dinero siempre en `DECIMAL(10,2)` / `decimal`. Los estados y métodos de pago se guardan como **texto** (`Pendiente`, `Yape`, ...), con `CHECK` en la BD.

## 5. Reglas ya resueltas en el tronco (no las reimplementes)

| Regla | Dónde está |
|---|---|
| Confirmar pedido = **una transacción** (pedido + detalles). Precios y disponibilidad se leen de la BD, no del carrito | `PedidoService.ConfirmarPedidoAsync` |
| Precio unitario histórico en el detalle | `Pedido.AgregarDetalle` |
| Cambios de estado permitidos: Pedido `Pendiente → Pagado/Anulado`; Pago `Registrado → Anulado` | `ReglasDeEstado` + `Pedido.CambiarEstado` |
| Registrar pago = **una transacción** con bloqueo; valida estado Pendiente, monto, cajero y método | `sp_registrar_pago` (vía `PagoRepository`) |
| **Sin pagos duplicados** | Índice único `UQ_Pago_IdPedido` + validación en el SP |
| Anular pedido sin pago, con control de concurrencia | `PedidoService.AnularPedidoAsync` |
| Reporte de ventas con fechas y paginación | `sp_reporte_ventas_paginado` |
| Errores de negocio | `DomainException` → mostrar con `TempData["Error"]` |

Si cambias la regla de estados, actualiza **los dos lugares**: `ReglasDeEstado.cs` y `sp_registrar_pago.sql`.

## 6. Contratos entre módulos (`Services/Interfaces`)

| Interfaz | Responsable | La usa |
|---|---|---|
| `ICurrentUserService`, `IAuthService`, `IPasswordHasherService`, `IClienteService` | Javier | Todos |
| `ICatalogoService` (solo lectura) | Valeria | Raúl |
| `ICarritoService` (sesión JSON), `IPedidoService` | Raúl | Montaño |
| `IPagoService`, `IReporteVentasService` | Montaño | Caja / Reportes |

Si necesitas **cambiar una interfaz existente**, avisa por el grupo y hazlo en un PR pequeño y aparte: la rompe a los demás.

## 7. Seguridad y roles

- Autenticación por **cookies**. Todo exige sesión iniciada salvo lo marcado con `[AllowAnonymous]` (política por defecto en `Program.cs`).
- Roles: `Administrador`, `Cajero`, `Cliente` (constantes en `Security/Roles.cs`). Uso: `[Authorize(Roles = Roles.Administrador)]`.
- Menú por rol en `Views/Shared/_Layout.cshtml`.
- Los formularios POST llevan `[ValidateAntiForgeryToken]` (los `<form asp-...>` agregan el token solos).
- Las vistas dentro de un Area usan `/Areas/_ViewStart.cshtml` y `/Areas/_ViewImports.cshtml`.

## 8. Qué falta en cada módulo

Los controladores de ejemplo ya muestran cómo conectar con los servicios. Las pantallas pendientes dicen "En construcción" (helper `EnConstruccion(...)`).

- **Javier — Seguridad:** registro y administración de usuarios, **CRUD de clientes** (no estaba asignado a nadie; se propone aquí porque Cliente se vincula con Usuario), registro de clientes con su usuario, cambio de contraseña, integración general.
- **Valeria — Catálogo:** CRUD de categorías y productos (desactivar en vez de borrar), control de disponibilidad, `ProductosController.Index` ya lista el catálogo.
- **Raúl — Pedidos:** agregar al carrito desde el catálogo, cambiar cantidades, quitar, confirmar (resolver el `IdCliente` con `IClienteService.ObtenerPorUsuarioAsync`), mis pedidos y detalle.
- **Montaño — Caja:** pantalla de cobro con `IPagoService.RegistrarPagoAsync`, comprobante interno imprimible, reporte de ventas con filtros y paginación (`IReporteVentasService`).

## 9. Convenciones

- Namespace = ruta de carpeta (`BiteFlow.Web.Areas.Caja.Controllers`).
- Cada controlador de Area lleva `[Area("...")]` y hereda de `ModuloControllerBase`.
- Los formularios usan **ViewModels** (`Models/ViewModels`), no entidades directamente.
- Servicios nuevos: se registran en **tu** método de `ServiceRegistration.cs` (`AddCatalogoModule`, etc.).
- Scripts JS propios: `wwwroot/js/<modulo>.js`.
- Nunca subir `bin/`, `obj/`, `.vs/` ni cadenas de conexión personales (ya están en `.gitignore`).

## 10. Cambios en la base de datos

La BD tiene **una sola fuente de verdad: `Database/01_schema.sql`** (no hay migraciones de EF Core, ver abajo). Para cambiar una tabla:
1. Edita `01_schema.sql` (con guardas `IF NOT EXISTS`) o agrega un script `Database/NN_cambio_descripcion.sql`.
2. Actualiza la entidad en `Models/Entities` y su `Data/Configurations/*Configuration.cs`.
3. Abre un PR que avise a todos para que ejecuten el script.

**Sobre las migraciones:** el tronco se generó sin poder ejecutar `dotnet ef`, así que **no incluye migraciones**. La maqueta del profesor sí las usa; si el curso las exige, Javier puede generarlas en su equipo (`Add-Migration InicialBiteFlow`) sobre una BD vacía, cuidando incluir en la migración el `sp_*`, la secuencia y los índices filtrados (`migrationBuilder.Sql(...)`), y desde ese momento las migraciones reemplazan a `01_schema.sql`. Decídanlo entre los cuatro antes de que alguien cree datos.

## 11. Verificación pendiente

Al generar este tronco **no se compiló ni se ejecutó nada**. Solo se hicieron comprobaciones estáticas: sintaxis de los 60 `.cs` con un analizador (no detecta errores de tipos ni de paquetes), coherencia de nombres entre controladores, vistas, menú, servicios y enums, validez del XML/JSON, formato del hash de contraseñas (calculado y verificado con Python) y un análisis superficial de los SQL con un analizador que no es de SQL Server y **no reconoce todo T-SQL** (no sustituye ejecutarlos). Las vistas Razor no se compilaron. Pendiente de comprobar en Visual Studio:

- [ ] La solución restaura paquetes y **compila**.
- [ ] Los scripts SQL se ejecutan en orden sin errores en LocalDB.
- [ ] Login con los tres usuarios de prueba y menú correcto para cada rol.
- [ ] `/Catalogo/Productos`, `/Pedidos/Carrito`, `/Caja/Pagos` cargan sin errores.
- [ ] Un pedido de prueba y su pago funcionan (se puede insertar un pedido a mano en SQL para probar `sp_registrar_pago`).

## 12. Problemas frecuentes

| Síntoma | Solución |
|---|---|
| `The certificate chain was issued by an authority that is not trusted` | Agrega `TrustServerCertificate=True;` a la cadena de conexión (en secretos de usuario) |
| `Cannot open database "BiteFlowDb"` | No ejecutaste `01_schema.sql` |
| `Invalid object name 'dbo.sp_registrar_pago'` | Falta ejecutar los archivos de `Database/Sp/` |
| Login siempre falla | No ejecutaste `02_seed.sql` (o cambió el formato del hash) |
| La app pide .NET 7 y no lo encuentra | Instala el SDK 7.0 (ver requisitos) |
