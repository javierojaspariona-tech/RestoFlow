using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteFlow.Web.Models;

public class Rol
{
    [Key]
    public int IdRol { get; set; }

    public string Nombre { get; set; } = "";

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}