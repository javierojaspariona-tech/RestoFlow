USE BiteFlowDb;
GO
-- Usuarios de prueba (ejecutar DESPUES de 02_seed.sql). Se puede ejecutar varias veces.
-- Contraseñas:  admin@biteflow.local = Admin123*   |   cajero@biteflow.local = Cajero123*   |   cliente@biteflow.local = Cliente123*
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'admin@biteflow.local')
    INSERT dbo.Usuario(IdRol, Nombres, Apellidos, Correo, PasswordHash, Activo, FechaCreacion)
    SELECT IdRol, N'Admin', N'BiteFlow', N'admin@biteflow.local', N'PBKDF2-SHA256$100000$Z+6MtaPYPK+FlWDlnEs7Dw==$tWkkk/iIrkEU3L0/USQ6uEyMUdgWcWrTf3q40NF3eCs=', 1, SYSDATETIME() FROM dbo.Rol WHERE Nombre = N'Administrador';
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'cajero@biteflow.local')
    INSERT dbo.Usuario(IdRol, Nombres, Apellidos, Correo, PasswordHash, Activo, FechaCreacion)
    SELECT IdRol, N'Caja', N'BiteFlow', N'cajero@biteflow.local', N'PBKDF2-SHA256$100000$a/MkURU5FYHiEtYNBBEJYw==$GZlTquiY8iTQ+8eree7pALzFcAzJEIKETICsVNa5E0k=', 1, SYSDATETIME() FROM dbo.Rol WHERE Nombre = N'Cajero';
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'cliente@biteflow.local')
    INSERT dbo.Usuario(IdRol, Nombres, Apellidos, Correo, PasswordHash, Activo, FechaCreacion)
    SELECT IdRol, N'Cliente', N'Demo', N'cliente@biteflow.local', N'PBKDF2-SHA256$100000$6QwJrRt2lxDa43TXox20SQ==$pfUl+upDQ2fbiv/7zae6a4d4K4KTh89oZqBO+zCr3HE=', 1, SYSDATETIME() FROM dbo.Rol WHERE Nombre = N'Cliente';
GO
-- El cliente necesita su fila en dbo.Cliente para poder hacer pedidos
IF NOT EXISTS (SELECT 1 FROM dbo.Cliente c JOIN dbo.Usuario u ON u.IdUsuario = c.IdUsuario WHERE u.Correo = N'cliente@biteflow.local')
    INSERT dbo.Cliente(IdUsuario, Telefono, Direccion)
    SELECT IdUsuario, N'999999999', N'Lima' FROM dbo.Usuario WHERE Correo = N'cliente@biteflow.local';
GO
SELECT u.IdUsuario, u.Correo, r.Nombre AS Rol, u.Activo FROM dbo.Usuario u JOIN dbo.Rol r ON r.IdRol = u.IdRol;
