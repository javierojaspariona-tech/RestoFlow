using BiteFlow.Web.Models;
using BiteFlow.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Controllers;
[Authorize(Roles=Roles.Administrador)]
public class CategoriasController : Controller
{
    private readonly ProductoRepository _s; public CategoriasController(ProductoRepository s)=>_s=s;
    public async Task<IActionResult> Index(CancellationToken ct)=>View(await _s.CategoriasAsync(ct));
    public IActionResult Create()=>View(new CategoriaViewModel());
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(CategoriaViewModel m,CancellationToken ct){if(!ModelState.IsValid)return View(m);try{await _s.GuardarCategoriaAsync(new Categoria{Nombre=m.Nombre,Descripcion=m.Descripcion},ct);TempData["Success"]="Categoría creada.";}catch(Exception ex) when(ex is InvalidOperationException or Microsoft.EntityFrameworkCore.DbUpdateException){TempData["Error"]="No se pudo guardar la categoría. Verifica que el nombre no esté repetido.";}return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Eliminar(int id,CancellationToken ct){try{await _s.EliminarCategoriaAsync(id,ct);TempData["Success"]="Categoría desactivada.";}catch(InvalidOperationException ex){TempData["Error"]=ex.Message;}return RedirectToAction(nameof(Index));}
}
