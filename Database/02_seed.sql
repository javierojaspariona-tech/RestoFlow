USE BiteFlowDb;
GO

/* =====================================================================
   Datos iniciales de BiteFlow (re-ejecutable: no duplica registros).
   USUARIOS DE PRUEBA (solo desarrollo) - contrasena de los tres: Biteflow123*
       admin@biteflow.local   (Administrador)
       cajero@biteflow.local  (Cajero)
       cliente@biteflow.local (Cliente, vinculado al cliente demo)
   Cambia o elimina estos usuarios antes de cualquier entrega final / despliegue.
   ===================================================================== */
SET NOCOUNT ON;

/* ---------- Roles (los nombres deben coincidir con Security/Roles.cs) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Rol WHERE Nombre = N'Administrador')
    INSERT INTO dbo.Rol (Nombre, Descripcion) VALUES (N'Administrador', N'Gestiona usuarios, catalogo y reportes');
IF NOT EXISTS (SELECT 1 FROM dbo.Rol WHERE Nombre = N'Cajero')
    INSERT INTO dbo.Rol (Nombre, Descripcion) VALUES (N'Cajero', N'Registra pagos y comprobantes');
IF NOT EXISTS (SELECT 1 FROM dbo.Rol WHERE Nombre = N'Cliente')
    INSERT INTO dbo.Rol (Nombre, Descripcion) VALUES (N'Cliente', N'Consulta el catalogo y realiza pedidos');

/* ---------- Usuarios de prueba (hash PBKDF2-SHA256, mismo formato que PasswordHasherService) ---------- */
DECLARE @IdRolAdmin  INT = (SELECT IdRol FROM dbo.Rol WHERE Nombre = N'Administrador');
DECLARE @IdRolCajero INT = (SELECT IdRol FROM dbo.Rol WHERE Nombre = N'Cajero');
DECLARE @IdRolClient INT = (SELECT IdRol FROM dbo.Rol WHERE Nombre = N'Cliente');

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'admin@biteflow.local')
    INSERT INTO dbo.Usuario (Nombres, Apellidos, Correo, PasswordHash, IdRol)
    VALUES (N'Administrador', N'BiteFlow', N'admin@biteflow.local', N'PBKDF2-SHA256$100000$/FFMesmJMMizUxFU4YIwNA==$oeVA7vGUHrrIktDPgQ8Cs6sun1TZfqyzA3UqKYqP6rw=', @IdRolAdmin);

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'cajero@biteflow.local')
    INSERT INTO dbo.Usuario (Nombres, Apellidos, Correo, PasswordHash, IdRol)
    VALUES (N'Cajero', N'Demo', N'cajero@biteflow.local', N'PBKDF2-SHA256$100000$5iHq8nKrdbEqSzCSNHnDgg==$LI8hVc62APNrVTPBRhj2P7DHD3z8DM0HhIXeRp2CuUM=', @IdRolCajero);

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'cliente@biteflow.local')
    INSERT INTO dbo.Usuario (Nombres, Apellidos, Correo, PasswordHash, IdRol)
    VALUES (N'Cliente', N'Demo', N'cliente@biteflow.local', N'PBKDF2-SHA256$100000$aiHBw2WOezr90sr86rq2Zw==$ct+/cmo0BwWEOrAhjHBjRqfdlwLa9TRF0WHu9/kV0jA=', @IdRolClient);

/* ---------- Cliente demo vinculado al usuario cliente@biteflow.local ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE Dni = '70000001')
    INSERT INTO dbo.Cliente (Nombres, Apellidos, Dni, Telefono, Correo, IdUsuario)
    VALUES (N'Cliente', N'Demo', '70000001', N'999000111', N'cliente@biteflow.local',
            (SELECT IdUsuario FROM dbo.Usuario WHERE Correo = N'cliente@biteflow.local'));

/* ---------- Categorias ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Categoria WHERE Nombre = N'Platos de fondo')
    INSERT INTO dbo.Categoria (Nombre, Descripcion) VALUES (N'Platos de fondo', N'Platos principales');
IF NOT EXISTS (SELECT 1 FROM dbo.Categoria WHERE Nombre = N'Bebidas')
    INSERT INTO dbo.Categoria (Nombre, Descripcion) VALUES (N'Bebidas', N'Refrescos y jugos');
IF NOT EXISTS (SELECT 1 FROM dbo.Categoria WHERE Nombre = N'Postres')
    INSERT INTO dbo.Categoria (Nombre, Descripcion) VALUES (N'Postres', N'Dulces y helados');

/* ---------- Productos de ejemplo ---------- */
DECLARE @IdFondo INT = (SELECT IdCategoria FROM dbo.Categoria WHERE Nombre = N'Platos de fondo');
DECLARE @IdBebida INT = (SELECT IdCategoria FROM dbo.Categoria WHERE Nombre = N'Bebidas');
DECLARE @IdPostre INT = (SELECT IdCategoria FROM dbo.Categoria WHERE Nombre = N'Postres');

IF NOT EXISTS (SELECT 1 FROM dbo.Producto WHERE IdCategoria = @IdFondo AND Nombre = N'Lomo saltado')
    INSERT INTO dbo.Producto (IdCategoria, Nombre, Descripcion, Precio) VALUES (@IdFondo, N'Lomo saltado', N'Lomo fino con papas fritas y arroz', 28.00);
IF NOT EXISTS (SELECT 1 FROM dbo.Producto WHERE IdCategoria = @IdFondo AND Nombre = N'Arroz con pollo')
    INSERT INTO dbo.Producto (IdCategoria, Nombre, Descripcion, Precio) VALUES (@IdFondo, N'Arroz con pollo', N'Arroz verde con presa de pollo y ensalada', 18.50);
IF NOT EXISTS (SELECT 1 FROM dbo.Producto WHERE IdCategoria = @IdFondo AND Nombre = N'Tallarines verdes')
    INSERT INTO dbo.Producto (IdCategoria, Nombre, Descripcion, Precio) VALUES (@IdFondo, N'Tallarines verdes', N'Con bistec a la parrilla', 24.00);
IF NOT EXISTS (SELECT 1 FROM dbo.Producto WHERE IdCategoria = @IdBebida AND Nombre = N'Chicha morada')
    INSERT INTO dbo.Producto (IdCategoria, Nombre, Descripcion, Precio) VALUES (@IdBebida, N'Chicha morada', N'Jarra personal', 6.00);
IF NOT EXISTS (SELECT 1 FROM dbo.Producto WHERE IdCategoria = @IdBebida AND Nombre = N'Limonada')
    INSERT INTO dbo.Producto (IdCategoria, Nombre, Descripcion, Precio) VALUES (@IdBebida, N'Limonada', N'Vaso grande', 5.50);
IF NOT EXISTS (SELECT 1 FROM dbo.Producto WHERE IdCategoria = @IdPostre AND Nombre = N'Suspiro limeno')
    INSERT INTO dbo.Producto (IdCategoria, Nombre, Descripcion, Precio) VALUES (@IdPostre, N'Suspiro limeno', N'Porcion individual', 9.00);
IF NOT EXISTS (SELECT 1 FROM dbo.Producto WHERE IdCategoria = @IdPostre AND Nombre = N'Helado de lucuma')
    INSERT INTO dbo.Producto (IdCategoria, Nombre, Descripcion, Precio, Disponible) VALUES (@IdPostre, N'Helado de lucuma', N'Dos bolas (no disponible: ejemplo de producto agotado)', 7.00, 0);

PRINT 'Datos iniciales cargados.';
GO
