using BiteFlow.Web.Models;
using BiteFlow.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BiteFlow.Web.Controllers;
[Authorize(Roles=Roles.Cliente)]
public class PedidosController : Controller
{
    private readonly PedidoRepository _pedidos; private readonly CarritoRepository _carrito;
    public PedidosController(PedidoRepository pedidos,CarritoRepository carrito){_pedidos=pedidos;_carrito=carrito;}
    private int ClienteId => int.TryParse(User.FindFirst("IdCliente")?.Value, out var id)
        ? id
        : throw new InvalidOperationException("La cuenta no tiene un perfil de cliente vinculado.");
    public async Task<IActionResult> MisPedidos(CancellationToken ct)=>View(await _pedidos.MisPedidosAsync(ClienteId,ct));
    public async Task<IActionResult> Detalle(long id,CancellationToken ct){var p=await _pedidos.ObtenerAsync(id,ct);if(p==null||p.IdCliente!=ClienteId)return NotFound();return View(p);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Confirmar(PedidoViewModel m,CancellationToken ct){if(!ModelState.IsValid)return RedirectToAction("Index","Carrito");try{var id=await _pedidos.ConfirmarAsync(ClienteId,m.Modalidad,m.NumeroMesa,m.Observacion,_carrito.Obtener(),ct);_carrito.Vaciar();TempData["Success"]=$"Pedido {id} registrado. Está pendiente de pago.";return RedirectToAction(nameof(MisPedidos));}catch(InvalidOperationException ex){TempData["Error"]=ex.Message;return RedirectToAction("Index","Carrito");}}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Anular(long id,CancellationToken ct){try{await _pedidos.AnularAsync(id,ClienteId,ct);TempData["Success"]="Pedido anulado.";}catch(InvalidOperationException ex){TempData["Error"]=ex.Message;}return RedirectToAction(nameof(MisPedidos));}
}
