namespace BiteFlow.Web.Models.Entities;

public class DetallePedido
{
    public int IdDetalle { get; set; }

    public int IdPedido { get; set; }
    public Pedido Pedido { get; set; } = null!;

    public int IdProducto { get; set; }
    public Producto Producto { get; set; } = null!;

    public int Cantidad { get; set; }

    /// <summary>Precio histórico: el que tenía el producto al momento de la compra.</summary>
    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }
}
