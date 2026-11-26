using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiteFlow.Web.Models;
public class Categoria
{
    [Key]
    public int IdCategoria { get; set; } public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; } public bool Activo { get; set; } = true;
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
