using BiteFlow.Web.Common;
using BiteFlow.Web.Models.Enums;

namespace BiteFlow.Web.Models.Entities;

public class Pedido
{
    public int IdPedido { get; set; }

    public int IdCliente { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public DateTime FechaPedido { get; set; } = DateTime.Now;

    /// <summary>Solo se modifica con <see cref="CambiarEstado"/> para respetar las transiciones permitidas.</summary>
    public EstadoPedido Estado { get; private set; } = EstadoPedido.Pendiente;

    public decimal Total { get; set; }

    /// <summary>Control de concurrencia: evita que dos operaciones simultáneas pisen el mismo pedido.</summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    public Pago? Pago { get; set; }

    public void CambiarEstado(EstadoPedido nuevoEstado)
    {
        if (!ReglasDeEstado.EsTransicionValida(Estado, nuevoEstado))
            throw new DomainException($"Cambio de estado no permitido: {Estado} -> {nuevoEstado}.");

        Estado = nuevoEstado;
    }

    /// <summary>
    /// Agrega (o suma) un producto al pedido guardando el PRECIO VIGENTE como precio histórico
    /// del detalle. Si el producto cambia de precio después, el pedido no se altera.
    /// </summary>
    public DetallePedido AgregarDetalle(Producto producto, int cantidad)
    {
        if (cantidad <= 0)
            throw new DomainException("La cantidad debe ser mayor que cero.");

        var detalle = Detalles.FirstOrDefault(d => d.IdProducto == producto.IdProducto);
        if (detalle is null)
        {
            detalle = new DetallePedido
            {
                IdProducto = producto.IdProducto,
                Producto = producto,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            };
            Detalles.Add(detalle);
        }
        else
        {
            detalle.Cantidad += cantidad;
        }

        detalle.Subtotal = Math.Round(detalle.PrecioUnitario * detalle.Cantidad, 2);
        RecalcularTotal();
        return detalle;
    }

    public void RecalcularTotal()
    {
        Total = Detalles.Sum(d => d.Subtotal);
    }
}
