using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiteFlow.Web.Models;
public class Cliente
{
    [Key]
    public int IdCliente { get; set; } public int IdUsuario { get; set; }
    public string? Telefono { get; set; } public string? Direccion { get; set; }
    [ForeignKey(nameof(IdUsuario))]
    public Usuario? Usuario { get; set; } public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
