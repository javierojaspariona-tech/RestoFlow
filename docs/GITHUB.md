# Guía de GitHub para el equipo BiteFlow

Cuatro integrantes, una sola solución, una sola base de datos. La rama **`main`** es el *tronco*: siempre debe compilar.

| Integrante | Rama de trabajo | Carpeta propia |
|---|---|---|
| Javier | `feature/seguridad` | `BiteFlow.Web/Areas/Seguridad` |
| Valeria | `feature/catalogo` | `BiteFlow.Web/Areas/Catalogo` |
| Raúl | `feature/pedidos` | `BiteFlow.Web/Areas/Pedidos` |
| Montaño | `feature/caja` | `BiteFlow.Web/Areas/Caja` |

## A. Una sola vez — Javier (coordinador)

1. Crea en GitHub un repositorio **privado** llamado `biteflow` (sin README ni .gitignore: ya vienen en el proyecto).
2. Desde la carpeta del proyecto (la que contiene `BiteFlow.sln`):
   ```bash
   git init -b main
   git add .
   git commit -m "Tronco inicial de BiteFlow"
   git remote add origin https://github.com/<usuario>/biteflow.git
   git push -u origin main
   ```
3. **Antes de empezar:** abre la solución, compila y ejecuta los scripts de `Database/`. Si todo funciona, avisa al grupo. Si no compila, corrígelo en `main` y avisa. Nadie crea su rama hasta entonces.
4. En *Settings > Collaborators* agrega a Valeria, Raúl y Montaño.
5. En *Settings > Branches > Add rule* para `main`: exige **Pull Request antes de fusionar** y 1 aprobación.

## B. Una sola vez — cada integrante

```bash
git clone https://github.com/<usuario>/biteflow.git
cd biteflow
git checkout -b feature/<tu-modulo>      # ej.: feature/catalogo
git push -u origin feature/<tu-modulo>
```
Luego sigue el README: ejecuta los scripts SQL y abre `BiteFlow.sln`.

## C. Flujo diario

```bash
# 1. Al empezar: trae lo último del tronco a tu rama
git checkout main
git pull
git checkout feature/<tu-modulo>
git merge main                   # resuelve conflictos aquí, en tu rama

# 2. Trabaja y haz commits pequeños y frecuentes
git add .
git commit -m "Catalogo: listado de productos con filtro por categoria"

# 3. Sube tu rama
git push
```

**Mensajes de commit:** `Modulo: qué hiciste` (ej. `Caja: registrar pago con comprobante`).

## D. Integrar al tronco (Pull Request)

1. Sube tu rama y abre un **Pull Request** hacia `main` en GitHub (se carga una plantilla con la lista de verificación).
2. Antes de abrirlo: `git merge main`, **compila** y prueba tu pantalla.
3. Otro integrante lo revisa (Javier revisa los que tocan archivos compartidos) y lo fusiona.
4. Después de fusionar, todos hacen `git checkout main && git pull` y `git merge main` en su rama.

Haz PR **pequeños y frecuentes** (cada pantalla o funcionalidad), no uno gigante al final.

## E. Qué NO tocar sin avisar (archivos compartidos)

Cambiarlos genera conflictos para todos. Si los necesitas, pide el cambio a Javier o haz un PR **solo con ese cambio**:

- `Program.cs`, `BiteFlow.Web.csproj`, `Views/Shared/_Layout.cshtml`
- `Data/AppDbContext.cs` y las `Data/Configurations/*` de entidades que no son tuyas
- `Models/Entities/*`, `Models/Enums/*`, `Security/Roles.cs`
- `Services/Interfaces/*` (cambiar un contrato rompe a los demás)
- `Database/01_schema.sql` (ver sección 10 del README)

Lo tuyo: tu Area, tus servicios nuevos y tu método en `ServiceRegistration.cs`.

## F. Conflictos

- Ocurren cuando dos personas editan las mismas líneas. Se resuelven **en tu rama** después de `git merge main`: Visual Studio abre el editor de combinación; elige qué queda, guarda, `git add .` y `git commit`.
- Si dudas, no borres lo del otro: pregunta.

## G. Reglas del equipo

- Nunca hagas `push` directo a `main` ni `push --force`.
- `main` siempre debe compilar.
- No subas `bin/`, `obj/`, `.vs/`, contraseñas ni cadenas de conexión personales.
- Cambios en la base de datos: script nuevo o edición de `01_schema.sql` **y** PR con aviso al grupo.
