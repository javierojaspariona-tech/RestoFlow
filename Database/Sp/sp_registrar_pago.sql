USE BiteFlowDb;
GO

/* =====================================================================
   sp_registrar_pago
   Registra el pago de un pedido en UNA transaccion:
     1) bloquea la fila del pedido (UPDLOCK/HOLDLOCK) para que dos cajeros no cobren a la vez
     2) valida: pedido existe, estado Pendiente, sin pago previo, monto = total, cajero y metodo validos
     3) inserta el Pago con su comprobante interno y pasa el pedido a Pagado
   Errores de negocio (se muestran al usuario): 50001..50007
   La regla de estado (Pendiente -> Pagado) es la misma de ReglasDeEstado.cs.
   ===================================================================== */
CREATE OR ALTER PROCEDURE dbo.sp_registrar_pago
(
    @IdPedido           INT,
    @IdUsuarioCajero    INT,
    @MetodoPago         VARCHAR(20),
    @Monto              DECIMAL(10,2),
    @IdPago             INT         OUTPUT,
    @NumeroComprobante  VARCHAR(20) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Estado VARCHAR(20);
        DECLARE @Total  DECIMAL(10,2);
        DECLARE @Numero INT;

        SELECT @Estado = Estado, @Total = Total
        FROM dbo.Pedido WITH (UPDLOCK, HOLDLOCK)
        WHERE IdPedido = @IdPedido;

        IF @Estado IS NULL
            THROW 50001, N'El pedido no existe.', 1;

        IF @Estado <> 'Pendiente'
            THROW 50002, N'Solo se puede cobrar un pedido en estado Pendiente.', 1;

        IF EXISTS (SELECT 1 FROM dbo.Pago WHERE IdPedido = @IdPedido)
            THROW 50003, N'El pedido ya tiene un pago registrado.', 1;

        IF @Monto <> @Total
            THROW 50004, N'El monto del pago debe ser igual al total del pedido.', 1;

        IF NOT EXISTS (SELECT 1
                       FROM dbo.Usuario u
                       INNER JOIN dbo.Rol r ON r.IdRol = u.IdRol
                       WHERE u.IdUsuario = @IdUsuarioCajero
                         AND u.Activo = 1
                         AND r.Nombre IN (N'Cajero', N'Administrador'))
            THROW 50005, N'El usuario no esta autorizado para registrar pagos.', 1;

        IF @MetodoPago NOT IN ('Efectivo', 'Yape', 'Plin', 'Tarjeta')
            THROW 50006, N'El metodo de pago no es valido.', 1;

        SET @Numero = NEXT VALUE FOR dbo.seq_numero_comprobante;
        SET @NumeroComprobante = 'INT-' + RIGHT('00000000' + CAST(@Numero AS VARCHAR(8)), 8);

        INSERT INTO dbo.Pago (IdPedido, IdUsuarioCajero, FechaPago, Monto, MetodoPago, Estado, NumeroComprobante)
        VALUES (@IdPedido, @IdUsuarioCajero, SYSDATETIME(), @Monto, @MetodoPago, 'Registrado', @NumeroComprobante);

        SET @IdPago = SCOPE_IDENTITY();

        UPDATE dbo.Pedido
        SET Estado = 'Pagado'
        WHERE IdPedido = @IdPedido
          AND Estado = 'Pendiente';

        IF @@ROWCOUNT <> 1
            THROW 50007, N'No se pudo actualizar el estado del pedido. Intenta nuevamente.', 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO
