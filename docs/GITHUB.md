# Guía de GitHub para el equipo BiteFlow

Cuatro integrantes trabajan sobre una sola solución. La rama `main` es la versión compartida y debe mantenerse compilable.

| Integrante | Rama | Módulo |
|---|---|---|
| Javier | `feature/seguridad` | Inicio de sesión, registro y usuarios |
| Valeria | `feature/catalogo` | Productos y categorías |
| Raúl | `feature/pedidos` | Carrito y pedidos |
| Montaño | `feature/caja` | Pagos y reportes |

## Clonar el repositorio

Cada integrante clona el repositorio una sola vez y crea su rama de trabajo. No debe crear un repositorio distinto para su módulo.

```bash
git clone <enlace-del-repositorio>
cd RestoFlow
git checkout -b feature/<modulo>
git push -u origin feature/<modulo>
```

## Flujo diario

1. Trae los cambios recientes de `main` (`git checkout main` y `git pull`).
2. Regresa a tu rama (`git checkout feature/<modulo>`) e incorpora `main` (`git merge main`).
3. Trabaja solo los archivos de tu módulo y crea commits pequeños y descriptivos.
4. Sube tu rama (`git push`) y abre un Pull Request hacia `main`.
5. Antes de integrar, vuelve a actualizar tu rama, compila y prueba la funcionalidad.

## Archivos compartidos

Coordina cualquier cambio en `Program.cs`, `BiteFlow.Web.csproj`, `Data/AppDbContext.cs`, `Views/Shared/_Layout.cshtml`, los modelos compartidos y los scripts de `BiteFlow.Web/Data/Sp/`.

## Reglas

- No hagas `push` directo a `main` ni uses `push --force`.
- No subas `bin/`, `obj/`, `.vs/`, contraseñas ni cadenas de conexión personales.
- Si una tarea requiere cambiar la base de datos, actualiza el script correspondiente y avisa al equipo en el Pull Request.
