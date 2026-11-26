
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteFlow.Web.Models;

public class Producto
{
    [Key]
    public int IdProducto { get; set; }

    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    public string? ImagenUrl { get; set; }
    public bool Disponible { get; set; } = true;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }

    [ForeignKey(nameof(IdCategoria))]
    public Categoria? Categoria { get; set; }

    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
}
