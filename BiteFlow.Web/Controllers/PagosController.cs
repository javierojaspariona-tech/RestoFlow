using BiteFlow.Web.Data;
using BiteFlow.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BiteFlow.Web.Controllers;

[Authorize(Roles = Roles.Administrador + "," + Roles.Cajero)]
public class PagosController : Controller
{
    private readonly PedidoRepository _pedidos;
    private readonly PagoRepository _pagos;
    public PagosController(PedidoRepository pedidos, PagoRepository pagos) { _pedidos = pedidos; _pagos = pagos; }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        // Si se acaba de cobrar, se carga el comprobante para mostrarlo en el simulador.
        if (TempData["CompPedidoId"] is string idTxt && long.TryParse(idTxt, out var idPedido))
        {
            ViewBag.Comprobante = await _pedidos.ComprobanteAsync(idPedido, ct);
            ViewBag.CompTipo = TempData["CompTipo"] as string ?? "Boleta";
            ViewBag.CompRuc = TempData["CompRuc"] as string;
            ViewBag.CompRazon = TempData["CompRazon"] as string;
        }
        return View(await _pedidos.PendientesAsync(ct));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(long id, string metodo, decimal monto, string? tipo, string? ruc, string? razon, CancellationToken ct)
    {
        tipo = tipo == "Factura" ? "Factura" : "Boleta";
        if (tipo == "Factura")
        {
            if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != 11 || !ruc.All(char.IsDigit))
            { TempData["Error"] = "El RUC debe tener 11 dígitos."; return RedirectToAction(nameof(Index)); }
            if (string.IsNullOrWhiteSpace(razon))
            { TempData["Error"] = "Ingresa la razón social para la factura."; return RedirectToAction(nameof(Index)); }
        }
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var r = await _pagos.RegistrarPagoAsync(id, userId, metodo, monto, ct);
            TempData["Success"] = $"Pago registrado. Comprobante: {r.NumeroComprobante}";
            TempData["CompPedidoId"] = id.ToString();
            TempData["CompTipo"] = tipo;
            TempData["CompRuc"] = ruc;
            TempData["CompRazon"] = razon;
        }
        catch (Exception ex) when (ex is InvalidOperationException or Microsoft.Data.SqlClient.SqlException)
        { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}
