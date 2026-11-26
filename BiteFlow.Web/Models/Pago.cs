
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteFlow.Web.Models;

public class Pago
{
    [Key]
    public long IdPago { get; set; }

    public long IdPedido { get; set; }
    public int IdUsuarioCajero { get; set; }
    public string MetodoPago { get; set; } = "";

    [Column(TypeName = "decimal(12,2)")]
    public decimal Monto { get; set; }

    public string Estado { get; set; } = "Registrado";
    public string NumeroComprobante { get; set; } = "";
    public DateTime FechaPago { get; set; }
    [ForeignKey(nameof(IdPedido))]
    public Pedido? Pedido { get; set; }
    [ForeignKey(nameof(IdUsuarioCajero))]
    public Usuario? UsuarioCajero { get; set; }
}
