namespace BiteFlow.Web.Models;
public class CarritoItem
{
    public int IdProducto { get; set; } public string Nombre { get; set; } = "";
    public decimal Precio { get; set; } public int Cantidad { get; set; } = 1;
    public string? ImagenUrl { get; set; }
    public decimal Importe => Precio * Cantidad;
}
