using System.Text.Json.Serialization;

namespace BiteFlow.Web.Models.Dtos;

/// <summary>
/// Línea del carrito (se guarda en sesión como JSON).
/// El precio aquí es solo informativo: al confirmar el pedido SIEMPRE se vuelve a leer de la BD.
/// </summary>
public class CarritoItem
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }

    [JsonIgnore]
    public decimal Subtotal => Math.Round(PrecioUnitario * Cantidad, 2);
}
