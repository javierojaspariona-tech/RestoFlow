using BiteFlow.Web.Models.Dtos;
using BiteFlow.Web.Models.Entities;

namespace BiteFlow.Web.Services.Interfaces;

/// <summary>[Pedidos - Raúl] Registro y consulta de pedidos. Lo consume Caja (Montaño).</summary>
public interface IPedidoService
{
    /// <summary>
    /// Registra el pedido y sus detalles en UNA transacción. Los precios se leen de la BD
    /// (no del carrito) y se guardan como precio histórico. Devuelve el IdPedido.
    /// Lanza DomainException si el carrito está vacío o algún producto ya no está disponible.
    /// </summary>
    Task<int> ConfirmarPedidoAsync(int idCliente, IReadOnlyList<CarritoItem> items, CancellationToken ct = default);

    Task<Pedido?> ObtenerPedidoAsync(int idPedido, CancellationToken ct = default);
    Task<IReadOnlyList<Pedido>> ListarPendientesDePagoAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Pedido>> ListarPorClienteAsync(int idCliente, CancellationToken ct = default);

    /// <summary>Anula un pedido Pendiente (sin pago). Respeta las transiciones de ReglasDeEstado.</summary>
    Task AnularPedidoAsync(int idPedido, CancellationToken ct = default);
}
