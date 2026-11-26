# Trabajo colaborativo de BiteFlow

El equipo trabaja sobre una sola solución ASP.NET Core MVC. Cada integrante debe desarrollar su módulo en una rama propia y mantener `main` estable.

| Integrante | Rama sugerida | Archivos principales |
|---|---|---|
| Javier | `feature/seguridad` | `Controllers/AccountController.cs`, `Controllers/UsuariosController.cs`, `Data/AuthRepository.cs`, `Views/Account/`, `Views/Usuarios/` |
| Valeria | `feature/catalogo` | `Controllers/ProductosController.cs`, `Controllers/CategoriasController.cs`, `Data/ProductoRepository.cs`, `Views/Productos/`, `Views/Categorias/` |
| Raúl | `feature/pedidos` | `Controllers/CarritoController.cs`, `Controllers/PedidosController.cs`, `Data/CarritoRepository.cs`, `Data/PedidoRepository.cs`, `Views/Carrito/`, `Views/Pedidos/` |
| Montaño | `feature/caja` | `Controllers/PagosController.cs`, `Controllers/ReportesController.cs`, `Data/PagoRepository.cs`, `Views/Pagos/`, `Views/Reportes/` |

## Antes de empezar

1. Abre `BiteFlow.sln` y restaura los paquetes NuGet.
2. Configura `ConnectionStrings:DefaultConnection` en `BiteFlow.Web/appsettings.json`.
3. Ejecuta en SQL Server los scripts de `BiteFlow.Web/Data/Sp/` en el orden indicado en `README.md`.
4. Compila y ejecuta el proyecto antes de comenzar a programar.

## Flujo de trabajo

1. Actualiza `main` con los últimos cambios (`git pull`).
2. Crea o cambia a tu rama `feature/<modulo>`.
3. Realiza commits pequeños que describan el avance, por ejemplo: `Catalogo: agregar filtro por categoría`.
4. Sube tu rama y abre un Pull Request hacia `main`.
5. Antes de integrar, actualiza tu rama con `main`, vuelve a compilar y prueba el módulo.

## Archivos compartidos

Coordina con el equipo antes de modificar `Program.cs`, `BiteFlow.Web.csproj`, `Data/AppDbContext.cs`, `Views/Shared/_Layout.cshtml`, los modelos usados por varios módulos o los scripts SQL. No subas `bin/`, `obj/`, `.vs/`, contraseñas ni cadenas de conexión personales.

No crees una segunda estructura paralela de controladores, áreas, servicios o entidades. El proyecto mantiene una única implementación en `Controllers/`, `Data/`, `Models/` y `Views/`.
