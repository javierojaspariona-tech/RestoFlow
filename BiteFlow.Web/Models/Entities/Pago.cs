using BiteFlow.Web.Models.Enums;

namespace BiteFlow.Web.Models.Entities;

public class Pago
{
    public int IdPago { get; set; }

    /// <summary>Un pedido admite como máximo UN pago (índice único UQ_Pago_IdPedido).</summary>
    public int IdPedido { get; set; }
    public Pedido Pedido { get; set; } = null!;

    public int IdUsuarioCajero { get; set; }
    public Usuario Cajero { get; set; } = null!;

    public DateTime FechaPago { get; set; } = DateTime.Now;
    public decimal Monto { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public EstadoPago Estado { get; set; } = EstadoPago.Registrado;

    /// <summary>Comprobante interno (sin validez tributaria). Ej.: INT-00000001.</summary>
    public string NumeroComprobante { get; set; } = string.Empty;
}
