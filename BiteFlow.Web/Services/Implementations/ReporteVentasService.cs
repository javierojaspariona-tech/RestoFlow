using BiteFlow.Web.Common;
using BiteFlow.Web.Data;
using BiteFlow.Web.Models.Dtos;
using BiteFlow.Web.Services.Interfaces;

namespace BiteFlow.Web.Services.Implementations;

public class ReporteVentasService : IReporteVentasService
{
    private readonly PagoRepository _pagoRepository;

    public ReporteVentasService(PagoRepository pagoRepository)
    {
        _pagoRepository = pagoRepository;
    }

    public Task<ReporteVentasResultado> BuscarAsync(ReporteVentasFiltro filtro, CancellationToken ct = default)
    {
        if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde.Value.Date > filtro.Hasta.Value.Date)
            throw new DomainException("La fecha inicial no puede ser mayor que la fecha final.");

        var pagina = Math.Max(1, filtro.Pagina);
        var tamano = Math.Clamp(filtro.TamanoPagina, 1, 100);

        return _pagoRepository.BuscarVentasSpAsync(filtro.Desde, filtro.Hasta, pagina, tamano, ct);
    }
}
