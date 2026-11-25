using BiteFlow.Web.Models.Enums;

namespace BiteFlow.Web.Models.Dtos;

public class ReporteVentasFiltro
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
}

public class VentaReporteItem
{
    public int IdPago { get; set; }
    public string NumeroComprobante { get; set; } = string.Empty;
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public int IdPedido { get; set; }
    public string Cliente { get; set; } = string.Empty;
}

public class ReporteVentasResultado
{
    public int Pagina { get; set; }
    public int TamanoPagina { get; set; }
    public int TotalRegistros { get; set; }
    public decimal TotalVendido { get; set; }
    public IReadOnlyList<VentaReporteItem> Items { get; set; } = Array.Empty<VentaReporteItem>();

    public int TotalPaginas =>
        TamanoPagina <= 0 ? 0 : (int)Math.Ceiling(TotalRegistros / (double)TamanoPagina);
}
