USE BiteFlowDb;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Rol)
BEGIN
    INSERT dbo.Rol(Nombre) VALUES (N'Administrador'), (N'Cajero'), (N'Cliente');
END;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Categoria)
BEGIN
    INSERT dbo.Categoria(Nombre, Descripcion, Activo) VALUES
    (N'Platos', N'Platos principales', 1),
    (N'Bebidas', N'Bebidas frías y calientes', 1),
    (N'Postres', N'Postres y dulces', 1);
END;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Producto)
BEGIN
    INSERT dbo.Producto(IdCategoria, Nombre, Descripcion, Precio, Disponible, Activo, FechaCreacion)
    SELECT c.IdCategoria, v.Nombre, v.Descripcion, v.Precio, 1, 1, SYSDATETIME()
    FROM (VALUES
      (N'Platos', N'Pollo a la plancha', N'Plato de demostración', CAST(22.00 AS DECIMAL(10,2))),
      (N'Bebidas', N'Limonada', N'Bebida de demostración', CAST(7.00 AS DECIMAL(10,2))),
      (N'Postres', N'Cheesecake', N'Postre de demostración', CAST(10.00 AS DECIMAL(10,2)))
    ) v(Categoria, Nombre, Descripcion, Precio)
    JOIN dbo.Categoria c ON c.Nombre = v.Categoria;
END;
GO
-- Usuarios de prueba: ejecutar 03_usuarios_prueba.sql
