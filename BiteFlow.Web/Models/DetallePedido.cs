
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteFlow.Web.Models;

public class DetallePedido
{
    [Key]
    public long IdDetallePedido { get; set; }

    public long IdPedido { get; set; }
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = "";
    public int Cantidad { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioUnitario { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public decimal Importe { get; private set; }
    [ForeignKey(nameof(IdPedido))]
    public Pedido? Pedido { get; set; }
    [ForeignKey(nameof(IdProducto))]
    public Producto? Producto { get; set; }
}
