using BiteFlow.Web.Models.Dtos;

namespace BiteFlow.Web.Services.Interfaces;

/// <summary>[Caja/Reportes - Montaño] Reporte de ventas con filtro por fechas y paginación.</summary>
public interface IReporteVentasService
{
    Task<ReporteVentasResultado> BuscarAsync(ReporteVentasFiltro filtro, CancellationToken ct = default);
}
