using BiteFlow.Web.Models;
using BiteFlow.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class ProductosController : Controller
{
    private readonly ProductoRepository _s;
    public ProductosController(ProductoRepository s) => _s = s;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.Categorias = (await _s.CategoriasAsync(ct)).Where(c => c.Activo).ToList();
        return View(await _s.ProductosAsync(ct));
    }

    // Crea (IdProducto = 0) o edita (IdProducto > 0) desde el modal.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(ProductoViewModel m, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index));
        }
        try
        {
            await _s.GuardarProductoAsync(new Producto { IdProducto = m.IdProducto, Nombre = m.Nombre, Descripcion = m.Descripcion, Precio = m.Precio!.Value, IdCategoria = m.IdCategoria!.Value, ImagenUrl = m.ImagenUrl, Disponible = m.Disponible }, ct);
            TempData["Success"] = m.IdProducto == 0 ? "Producto creado." : "Producto actualizado.";
        }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        try { await _s.EliminarProductoAsync(id, ct); TempData["Success"] = "Producto desactivado."; }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}
