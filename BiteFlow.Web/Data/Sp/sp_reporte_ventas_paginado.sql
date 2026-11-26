USE BiteFlowDb;
GO
CREATE OR ALTER PROCEDURE dbo.sp_reporte_ventas_paginado
    @FechaInicio DATE,
    @FechaFin DATE,
    @Pagina INT = 1,
    @TamanoPagina INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    IF @FechaInicio IS NULL OR @FechaFin IS NULL OR @FechaFin < @FechaInicio
        THROW 51101, 'Rango de fechas inválido.', 1;
    IF @Pagina < 1 SET @Pagina = 1;
    IF @TamanoPagina < 1 OR @TamanoPagina > 100 SET @TamanoPagina = 10;

    ;WITH Ventas AS (
        SELECT p.IdPago, pe.Codigo AS CodigoPedido, p.NumeroComprobante,
               p.FechaPago, p.MetodoPago, p.Monto,
               CONCAT(u.Nombres, N' ', u.Apellidos) AS Cajero,
               COUNT(*) OVER() AS TotalRegistros
        FROM dbo.Pago p
        JOIN dbo.Pedido pe ON pe.IdPedido = p.IdPedido
        JOIN dbo.Usuario u ON u.IdUsuario = p.IdUsuarioCajero
        WHERE p.Estado = N'Registrado'
          AND p.FechaPago >= @FechaInicio
          AND p.FechaPago < DATEADD(DAY, 1, @FechaFin)
    )
    SELECT *
    FROM Ventas
    ORDER BY FechaPago DESC, IdPago DESC
    OFFSET (@Pagina - 1) * @TamanoPagina ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY;
END;
GO
