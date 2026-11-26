namespace BiteFlow.Web.Models;
public class ResultadoPago
{
    public long IdPago { get; set; } public long IdPedido { get; set; }
    public decimal Monto { get; set; } public string MetodoPago { get; set; } = "";
    public string NumeroComprobante { get; set; } = ""; public DateTime FechaPago { get; set; }
}
