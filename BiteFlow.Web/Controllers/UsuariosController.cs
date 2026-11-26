using BiteFlow.Web.Data;
using BiteFlow.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Controllers;
[Authorize(Roles=Roles.Administrador)]
public class UsuariosController : Controller
{
    private readonly UsuarioRepository _service; public UsuariosController(UsuarioRepository service)=>_service=service;
    public async Task<IActionResult> Index(CancellationToken ct)=>View(await _service.ListarAsync(ct));
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> CambiarEstado(int id,CancellationToken ct){try{await _service.CambiarEstadoAsync(id,ct);TempData["Success"]="Estado actualizado.";}catch(InvalidOperationException ex){TempData["Error"]=ex.Message;}return RedirectToAction(nameof(Index));}
}
