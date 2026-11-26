/*
  BiteFlow - Repara valores por defecto que falten en las tablas.
  Úsalo si sale "Cannot insert the value NULL into column 'Activo'...".
  Se puede ejecutar varias veces; solo agrega lo que falta.
*/
USE BiteFlowDb;
GO
DECLARE @d TABLE (Tabla SYSNAME, Columna SYSNAME, Valor NVARCHAR(50));
INSERT @d VALUES
 (N'Usuario',   N'Activo',         N'1'),
 (N'Usuario',   N'FechaCreacion',  N'SYSDATETIME()'),
 (N'Categoria', N'Activo',         N'1'),
 (N'Producto',  N'Disponible',     N'1'),
 (N'Producto',  N'Activo',         N'1'),
 (N'Producto',  N'FechaCreacion',  N'SYSDATETIME()'),
 (N'Pedido',    N'Estado',         N'N''Pendiente'''),
 (N'Pedido',    N'FechaPedido',    N'SYSDATETIME()'),
 (N'Pago',      N'Estado',         N'N''Registrado'''),
 (N'Pago',      N'FechaPago',      N'SYSDATETIME()');

DECLARE @t SYSNAME, @c SYSNAME, @v NVARCHAR(50), @sql NVARCHAR(400);
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT Tabla, Columna, Valor FROM @d;
OPEN cur; FETCH NEXT FROM cur INTO @t, @c, @v;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints dc
        JOIN sys.columns col ON col.object_id = dc.parent_object_id AND col.column_id = dc.parent_column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'dbo.' + @t) AND col.name = @c)
    BEGIN
        SET @sql = N'ALTER TABLE dbo.' + QUOTENAME(@t) + N' ADD CONSTRAINT ' + QUOTENAME(N'DF_' + @t + N'_' + @c)
                 + N' DEFAULT (' + @v + N') FOR ' + QUOTENAME(@c) + N';';
        EXEC (@sql);
        PRINT N'Default agregado: ' + @t + N'.' + @c;
    END
    FETCH NEXT FROM cur INTO @t, @c, @v;
END
CLOSE cur; DEALLOCATE cur;
GO
