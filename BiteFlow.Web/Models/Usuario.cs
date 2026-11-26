using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiteFlow.Web.Models;
public class Usuario
{
    [Key]
    public int IdUsuario { get; set; } public int IdRol { get; set; }
    public string Nombres { get; set; } = ""; public string Apellidos { get; set; } = "";
    public string Correo { get; set; } = ""; public string PasswordHash { get; set; } = "";
    public bool Activo { get; set; } = true; public DateTime FechaCreacion { get; set; }
    [ForeignKey(nameof(IdRol))]
    public Rol? Rol { get; set; }

    public Cliente? Cliente { get; set; }
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
