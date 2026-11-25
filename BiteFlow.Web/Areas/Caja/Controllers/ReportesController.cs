using BiteFlow.Web.Common;
using BiteFlow.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Areas.Caja.Controllers;

[Area("Caja")]
[Authorize(Roles = Roles.Administrador)]
public class ReportesController : ModuloControllerBase
{
    public IActionResult Index()
    {
        return EnConstruccion("Reportes de ventas", "Montaño",
            "Filtros por fechas y paginación (IReporteVentasService.BuscarAsync, ya conectado a sp_reporte_ventas_paginado)",
            "Vista y exportación del reporte");
    }
}
