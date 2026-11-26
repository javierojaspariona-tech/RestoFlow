using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class ProductoViewModel
{
    public int IdProducto { get; set; }
    [Required(ErrorMessage = "Ingresa el nombre del producto."), StringLength(120, ErrorMessage = "Máximo 120 caracteres.")]
    public string Nombre { get; set; } = "";
    [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
    public string? Descripcion { get; set; }
    [Required(ErrorMessage = "Ingresa el precio.")]
    [Range(typeof(decimal), "0.01", "999999", ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal? Precio { get; set; }
    [Required(ErrorMessage = "Selecciona una categoría."), Display(Name = "Categoría")]
    public int? IdCategoria { get; set; }
    [Display(Name = "URL de imagen"), StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
    public string? ImagenUrl { get; set; }
    public bool Disponible { get; set; } = true;
}
