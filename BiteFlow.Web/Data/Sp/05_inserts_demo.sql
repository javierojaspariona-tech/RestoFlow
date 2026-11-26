/*
  BiteFlow - INSERTS de datos de demostración
  Orden: 01_schema -> sp_registrar_pago -> sp_reporte_ventas_paginado -> 02_seed -> 03_usuarios_prueba -> ESTE
  Se puede ejecutar varias veces: no duplica nada.
*/
USE BiteFlowDb;
GO

/* ===== CATEGORIA ===== */
INSERT dbo.Categoria(Nombre, Descripcion, Activo)
SELECT v.Nombre, v.Descripcion, 1
FROM (VALUES
    (N'Entradas', N'Para empezar')
) v(Nombre, Descripcion)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Categoria c WHERE c.Nombre = v.Nombre);
GO

/* ===== PRODUCTO ===== */
INSERT dbo.Producto(IdCategoria, Nombre, Descripcion, Precio, Disponible, Activo, FechaCreacion)
SELECT c.IdCategoria, v.Nombre, v.Descripcion, v.Precio, 1, 1, SYSDATETIME()
FROM (VALUES
    (N'Entradas', N'Papa a la Huancaína',          N'Papa sancochada con crema de ají amarillo y huevo',        CAST(14.00 AS DECIMAL(10,2))),
    (N'Entradas', N'Anticuchos',                   N'Brochetas de corazón con papa y ají',                      CAST(18.00 AS DECIMAL(10,2))),
    (N'Entradas', N'Tequeños',                     N'Ocho piezas con guacamole',                                CAST(15.00 AS DECIMAL(10,2))),
    (N'Platos',   N'Lomo Saltado',                 N'Lomo fino salteado con cebolla, tomate, papas y arroz',    CAST(28.00 AS DECIMAL(10,2))),
    (N'Platos',   N'Ají de Gallina',               N'Gallina deshilachada en crema de ají amarillo con arroz',  CAST(24.00 AS DECIMAL(10,2))),
    (N'Platos',   N'Tallarines Verdes con Bistec', N'Tallarines al pesto peruano con bistec a la plancha',      CAST(25.00 AS DECIMAL(10,2))),
    (N'Platos',   N'Arroz con Mariscos',           N'Arroz meloso con mariscos frescos',                        CAST(32.00 AS DECIMAL(10,2))),
    (N'Bebidas',  N'Chicha Morada',                N'Jarra personal de chicha morada',                          CAST(8.00 AS DECIMAL(10,2))),
    (N'Bebidas',  N'Maracuyá',                     N'Jugo natural de maracuyá',                                 CAST(8.00 AS DECIMAL(10,2))),
    (N'Bebidas',  N'Inca Kola 500ml',              N'Gaseosa personal',                                         CAST(6.00 AS DECIMAL(10,2))),
    (N'Bebidas',  N'Café Americano',               N'Café recién preparado',                                    CAST(6.00 AS DECIMAL(10,2))),
    (N'Postres',  N'Suspiro a la Limeña',          N'Manjar blanco con merengue al oporto',                     CAST(12.00 AS DECIMAL(10,2))),
    (N'Postres',  N'Tres Leches',                  N'Bizcocho bañado en tres leches',                           CAST(11.00 AS DECIMAL(10,2))),
    (N'Postres',  N'Helado (2 bolas)',             N'Vainilla, chocolate o fresa',                              CAST(9.00 AS DECIMAL(10,2)))
) v(Categoria, Nombre, Descripcion, Precio)
JOIN dbo.Categoria c ON c.Nombre = v.Categoria
WHERE NOT EXISTS (SELECT 1 FROM dbo.Producto p WHERE p.Nombre = v.Nombre);
GO

/* ===== PEDIDO + DETALLEPEDIDO + PAGO (requiere 03_usuarios_prueba.sql) ===== */
DECLARE @cli INT = (SELECT c.IdCliente FROM dbo.Cliente c JOIN dbo.Usuario u ON u.IdUsuario = c.IdUsuario WHERE u.Correo = N'cliente@biteflow.local');
DECLARE @caj INT = (SELECT IdUsuario FROM dbo.Usuario WHERE Correo = N'cajero@biteflow.local');
IF @cli IS NULL OR @caj IS NULL
    THROW 50001, 'Ejecuta primero 03_usuarios_prueba.sql (faltan el cliente o el cajero de prueba).', 1;

IF NOT EXISTS (SELECT 1 FROM dbo.Pedido WHERE Codigo LIKE N'DEMO-%')
BEGIN
    -- Pedidos (los montos se calculan después desde el detalle)
    INSERT dbo.Pedido(IdCliente, Codigo, Modalidad, NumeroMesa, Estado, Subtotal, Igv, Total, FechaPedido, Observacion) VALUES
    (@cli, N'DEMO-001', N'ParaLlevar', NULL, N'Pendiente', 0, 0, 0, DATEADD(DAY, -3, SYSDATETIME()), NULL),
    (@cli, N'DEMO-002', N'Mesa',       4,    N'Pendiente', 0, 0, 0, DATEADD(DAY, -2, SYSDATETIME()), N'Sin ají, por favor'),
    (@cli, N'DEMO-003', N'ParaLlevar', NULL, N'Pendiente', 0, 0, 0, DATEADD(DAY, -1, SYSDATETIME()), NULL),
    (@cli, N'DEMO-004', N'Mesa',       5,    N'Pendiente', 0, 0, 0, SYSDATETIME(),                   NULL),
    (@cli, N'DEMO-005', N'ParaLlevar', NULL, N'Pendiente', 0, 0, 0, SYSDATETIME(),                   N'Para recoger en 15 min'),
    (@cli, N'DEMO-006', N'ParaLlevar', NULL, N'Anulado',   0, 0, 0, DATEADD(DAY, -1, SYSDATETIME()), N'Cliente canceló');

    -- Detalle
    INSERT dbo.DetallePedido(IdPedido, IdProducto, NombreProducto, Cantidad, PrecioUnitario)
    SELECT pe.IdPedido, pr.IdProducto, pr.Nombre, v.Cantidad, pr.Precio
    FROM (VALUES
        (N'DEMO-001', N'Lomo Saltado', 2),
        (N'DEMO-001', N'Chicha Morada', 2),
        (N'DEMO-002', N'Ají de Gallina', 1),
        (N'DEMO-002', N'Limonada', 1),
        (N'DEMO-002', N'Suspiro a la Limeña', 1),
        (N'DEMO-003', N'Arroz con Mariscos', 1),
        (N'DEMO-003', N'Maracuyá', 1),
        (N'DEMO-004', N'Tallarines Verdes con Bistec', 2),
        (N'DEMO-004', N'Inca Kola 500ml', 2),
        (N'DEMO-004', N'Tres Leches', 1),
        (N'DEMO-005', N'Anticuchos', 1),
        (N'DEMO-005', N'Café Americano', 1),
        (N'DEMO-006', N'Tequeños', 1),
        (N'DEMO-006', N'Helado (2 bolas)', 1)
    ) v(Codigo, Producto, Cantidad)
    JOIN dbo.Pedido pe   ON pe.Codigo = v.Codigo
    JOIN dbo.Producto pr ON pr.Nombre = v.Producto;

    -- Totales (precios con IGV incluido: IGV = Total * 18/118)
    UPDATE pe
    SET Total    = t.Total,
        Igv      = ROUND(t.Total * 18 / 118, 2),
        Subtotal = t.Total - ROUND(t.Total * 18 / 118, 2)
    FROM dbo.Pedido pe
    JOIN (SELECT IdPedido, SUM(Importe) AS Total FROM dbo.DetallePedido GROUP BY IdPedido) t ON t.IdPedido = pe.IdPedido
    WHERE pe.Codigo LIKE N'DEMO-%';

    -- Pagos de los 3 primeros pedidos (usa el mismo procedimiento que la web)
    DECLARE @id BIGINT, @total DECIMAL(12,2);

    SELECT @id = IdPedido, @total = Total FROM dbo.Pedido WHERE Codigo = N'DEMO-001';
    EXEC dbo.sp_registrar_pago @id, @caj, N'Efectivo', @total;
    SELECT @id = IdPedido, @total = Total FROM dbo.Pedido WHERE Codigo = N'DEMO-002';
    EXEC dbo.sp_registrar_pago @id, @caj, N'YapePlin', @total;
    SELECT @id = IdPedido, @total = Total FROM dbo.Pedido WHERE Codigo = N'DEMO-003';
    EXEC dbo.sp_registrar_pago @id, @caj, N'Tarjeta', @total;

    -- Fecha del pago = 20 min después del pedido (para que el reporte por fechas tenga sentido)
    UPDATE pg SET FechaPago = DATEADD(MINUTE, 20, pe.FechaPedido)
    FROM dbo.Pago pg JOIN dbo.Pedido pe ON pe.IdPedido = pg.IdPedido
    WHERE pe.Codigo LIKE N'DEMO-%';
END;
GO

SELECT Codigo, Modalidad, NumeroMesa, Estado, Subtotal, Igv, Total FROM dbo.Pedido WHERE Codigo LIKE N'DEMO-%' ORDER BY Codigo;
GO
