namespace BiteFlow.Web.Models.Enums;

/// <summary>Estados del pedido. En la BD se guardan como texto (Pendiente, Pagado, Anulado).</summary>
public enum EstadoPedido
{
    Pendiente = 1,
    Pagado = 2,
    Anulado = 3
}
