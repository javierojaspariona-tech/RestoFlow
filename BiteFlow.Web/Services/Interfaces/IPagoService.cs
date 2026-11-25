using BiteFlow.Web.Models.Dtos;
using BiteFlow.Web.Models.Enums;

namespace BiteFlow.Web.Services.Interfaces;

/// <summary>[Caja - Montaño] Registro de pagos.</summary>
public interface IPagoService
{
    /// <summary>
    /// Registra el pago de un pedido Pendiente (monto = total del pedido), en una transacción:
    /// inserta el Pago, genera el comprobante interno y marca el pedido como Pagado.
    /// El cajero es el usuario con la sesión iniciada. Lanza DomainException si el pedido no existe,
    /// no está Pendiente o ya tiene un pago (pago duplicado).
    /// </summary>
    Task<ResultadoPago> RegistrarPagoAsync(int idPedido, MetodoPago metodoPago, CancellationToken ct = default);
}
