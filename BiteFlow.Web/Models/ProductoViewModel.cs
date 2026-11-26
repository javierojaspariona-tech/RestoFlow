using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class ProductoViewModel
{
    public int IdProducto { get; set; }
    [Required, StringLength(120)] public string Nombre { get; set; } = "";
    [StringLength(500)] public string? Descripcion { get; set; }
    [Range(0, 999999)] public decimal Precio { get; set; }
    [Required] public int IdCategoria { get; set; }
    public string? ImagenUrl { get; set; } public bool Disponible { get; set; } = true;
}
