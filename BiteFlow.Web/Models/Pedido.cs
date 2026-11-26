
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteFlow.Web.Models;

public class Pedido
{
    [Key]
    public long IdPedido { get; set; }

    public int IdCliente { get; set; }
    public string Codigo { get; set; } = "";
    public string Modalidad { get; set; } = "ParaLlevar";
    public byte? NumeroMesa { get; set; }
    public string Estado { get; set; } = "Pendiente";

    [Column(TypeName = "decimal(12,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Igv { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Total { get; set; }

    public DateTime FechaPedido { get; set; }
    public string? Observacion { get; set; }

    [ForeignKey(nameof(IdCliente))] 
    public Cliente? Cliente { get; set; }
    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    public Pago? Pago { get; set; }
}
