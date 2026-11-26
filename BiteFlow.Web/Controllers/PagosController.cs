using BiteFlow.Web.Data;
using BiteFlow.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BiteFlow.Web.Controllers;
[Authorize(Roles=Roles.Administrador+","+Roles.Cajero)]
public class PagosController : Controller
{
    private readonly PedidoRepository _pedidos; private readonly PagoRepository _pagos;
    public PagosController(PedidoRepository pedidos,PagoRepository pagos){_pedidos=pedidos;_pagos=pagos;}
    public async Task<IActionResult> Index(CancellationToken ct)=>View(await _pedidos.PendientesAsync(ct));
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Registrar(long id,string metodo,decimal monto,CancellationToken ct){try{var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);var r=await _pagos.RegistrarPagoAsync(id,userId,metodo,monto,ct);TempData["Success"]=$"Pago registrado. Comprobante: {r.NumeroComprobante}";}catch(Exception ex) when(ex is InvalidOperationException or Microsoft.Data.SqlClient.SqlException){TempData["Error"]=ex.Message;}return RedirectToAction(nameof(Index));}
}
