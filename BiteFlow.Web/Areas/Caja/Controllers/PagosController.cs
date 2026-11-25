using BiteFlow.Web.Common;
using BiteFlow.Web.Security;
using BiteFlow.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Areas.Caja.Controllers;

/// <summary>
/// Ejemplo de referencia: lista los pedidos pendientes de pago (IPedidoService).
/// Montaño agrega: pantalla de cobro, IPagoService.RegistrarPagoAsync y el comprobante imprimible.
/// </summary>
[Area("Caja")]
[Authorize(Roles = Roles.AdministradorOCajero)]
public class PagosController : ModuloControllerBase
{
    private readonly IPedidoService _pedidos;

    public PagosController(IPedidoService pedidos)
    {
        _pedidos = pedidos;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var pendientes = await _pedidos.ListarPendientesDePagoAsync(ct);
        return View(pendientes);
    }
}
