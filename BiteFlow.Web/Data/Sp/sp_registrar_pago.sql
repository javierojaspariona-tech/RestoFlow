USE BiteFlowDb;
GO
CREATE OR ALTER PROCEDURE dbo.sp_registrar_pago
    @IdPedido BIGINT,
    @IdUsuarioCajero INT,
    @MetodoPago NVARCHAR(20),
    @Monto DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Total DECIMAL(12,2), @Estado NVARCHAR(15), @Numero NVARCHAR(24);
    BEGIN TRANSACTION;

    SELECT @Total = Total, @Estado = Estado
    FROM dbo.Pedido WITH (UPDLOCK, HOLDLOCK)
    WHERE IdPedido = @IdPedido;

    IF @Total IS NULL
        THROW 51001, 'El pedido no existe.', 1;
    IF @Estado <> N'Pendiente'
        THROW 51002, 'El pedido no está pendiente o ya fue cobrado.', 1;
    IF ABS(@Monto - @Total) > 0.01
        THROW 51003, 'El monto no coincide con el total del pedido.', 1;
    IF @MetodoPago NOT IN (N'Efectivo', N'YapePlin', N'Tarjeta')
        THROW 51004, 'Método de pago no válido.', 1;
    IF NOT EXISTS (
        SELECT 1 FROM dbo.Usuario u
        JOIN dbo.Rol r ON r.IdRol = u.IdRol
        WHERE u.IdUsuario = @IdUsuarioCajero AND u.Activo = 1
          AND r.Nombre IN (N'Cajero', N'Administrador')
    )
        THROW 51005, 'El usuario no está autorizado para cobrar.', 1;

    SET @Numero = CONCAT(N'INT-', RIGHT(REPLICATE('0', 8) +
        CONVERT(VARCHAR(20), NEXT VALUE FOR dbo.SeqComprobante), 8));

    INSERT dbo.Pago(IdPedido, IdUsuarioCajero, MetodoPago, Monto, NumeroComprobante)
    VALUES (@IdPedido, @IdUsuarioCajero, @MetodoPago, @Monto, @Numero);

    UPDATE dbo.Pedido SET Estado = N'Pagado' WHERE IdPedido = @IdPedido;

    COMMIT TRANSACTION;

    SELECT p.IdPago, p.IdPedido, p.Monto, p.MetodoPago, p.NumeroComprobante, p.FechaPago
    FROM dbo.Pago p WHERE p.NumeroComprobante = @Numero;
END;
GO
