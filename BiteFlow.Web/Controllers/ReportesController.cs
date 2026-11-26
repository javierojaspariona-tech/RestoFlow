using BiteFlow.Web.Data;
using BiteFlow.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Controllers;
[Authorize(Roles=Roles.Administrador)]
public class ReportesController : Controller
{
    private readonly PagoRepository _s; public ReportesController(PagoRepository s)=>_s=s;
    public async Task<IActionResult> Index(DateTime? inicio,DateTime? fin,int pagina=1,CancellationToken ct=default){var desde=inicio?.Date??DateTime.Today.AddDays(-30);var hasta=fin?.Date??DateTime.Today;ViewBag.Inicio=desde.ToString("yyyy-MM-dd");ViewBag.Fin=hasta.ToString("yyyy-MM-dd");try{return View(await _s.ReporteAsync(desde,hasta,pagina,10,ct));}catch(InvalidOperationException ex){TempData["Error"]=ex.Message;return View(new BiteFlow.Web.Models.PaginaResultado<BiteFlow.Web.Models.ReporteVentaDto>{Pagina=pagina,TamanoPagina=10});}}
}
