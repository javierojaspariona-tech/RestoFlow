namespace BiteFlow.Web.Models;
public class ReporteVentaDto
{
    public long IdPago { get; set; } public string CodigoPedido { get; set; } = "";
    public string NumeroComprobante { get; set; } = ""; public DateTime FechaPago { get; set; }
    public string MetodoPago { get; set; } = ""; public decimal Monto { get; set; }
    public string Cajero { get; set; } = ""; public int TotalRegistros { get; set; }
}
