USE BiteFlowDb;
GO

/* =====================================================================
   sp_reporte_ventas_paginado
   Reporte de ventas (solo pagos Registrados) con filtro por fechas y paginacion.
   Devuelve DOS resultados:
     1) TotalRegistros y TotalVendido del filtro completo
     2) Las filas de la pagina solicitada
   @Desde / @Hasta son opcionales (NULL = sin limite). @Hasta es inclusivo (todo el dia).
   ===================================================================== */
CREATE OR ALTER PROCEDURE dbo.sp_reporte_ventas_paginado
(
    @Desde     DATE = NULL,
    @Hasta     DATE = NULL,
    @Page      INT  = 1,
    @PageSize  INT  = 10
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 10;
    IF @PageSize > 100 SET @PageSize = 100;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;
    DECLARE @HastaExclusivo DATETIME2 = CASE WHEN @Hasta IS NULL THEN NULL
                                             ELSE DATEADD(DAY, 1, CAST(@Hasta AS DATETIME2)) END;

    SELECT COUNT(*)                 AS TotalRegistros,
           ISNULL(SUM(p.Monto), 0)  AS TotalVendido
    FROM dbo.Pago p
    WHERE p.Estado = 'Registrado'
      AND (@Desde IS NULL OR p.FechaPago >= @Desde)
      AND (@HastaExclusivo IS NULL OR p.FechaPago < @HastaExclusivo);

    SELECT p.IdPago,
           p.NumeroComprobante,
           p.FechaPago,
           p.Monto,
           p.MetodoPago,
           p.IdPedido,
           c.Nombres + N' ' + c.Apellidos AS Cliente
    FROM dbo.Pago p
    INNER JOIN dbo.Pedido ped ON ped.IdPedido = p.IdPedido
    INNER JOIN dbo.Cliente c  ON c.IdCliente = ped.IdCliente
    WHERE p.Estado = 'Registrado'
      AND (@Desde IS NULL OR p.FechaPago >= @Desde)
      AND (@HastaExclusivo IS NULL OR p.FechaPago < @HastaExclusivo)
    ORDER BY p.FechaPago DESC, p.IdPago DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
