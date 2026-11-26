using BiteFlow.Web.Models;
using BiteFlow.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Controllers;
[Authorize(Roles=Roles.Administrador)]
public class ProductosController : Controller
{
    private readonly ProductoRepository _s; public ProductosController(ProductoRepository s)=>_s=s;
    public async Task<IActionResult> Index(CancellationToken ct)=>View(await _s.ProductosAsync(ct));
    public async Task<IActionResult> Create(CancellationToken ct){ViewBag.Categorias=await _s.CategoriasAsync(ct);return View(new ProductoViewModel());}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(ProductoViewModel m,CancellationToken ct){if(!ModelState.IsValid){ViewBag.Categorias=await _s.CategoriasAsync(ct);return View(m);}try{await _s.GuardarProductoAsync(new Producto{Nombre=m.Nombre,Descripcion=m.Descripcion,Precio=m.Precio,IdCategoria=m.IdCategoria,ImagenUrl=m.ImagenUrl,Disponible=m.Disponible},ct);TempData["Success"]="Producto creado.";}catch(InvalidOperationException ex){TempData["Error"]=ex.Message;}return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Eliminar(int id,CancellationToken ct){try{await _s.EliminarProductoAsync(id,ct);TempData["Success"]="Producto desactivado.";}catch(InvalidOperationException ex){TempData["Error"]=ex.Message;}return RedirectToAction(nameof(Index));}
}
