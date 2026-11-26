using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class CategoriaViewModel
{
    public int IdCategoria { get; set; }
    [Required(ErrorMessage = "Ingresa el nombre de la categoría."), StringLength(80, ErrorMessage = "Máximo 80 caracteres.")]
    public string Nombre { get; set; } = "";
    [Display(Name = "Descripción"), StringLength(250, ErrorMessage = "Máximo 250 caracteres.")]
    public string? Descripcion { get; set; }
}
