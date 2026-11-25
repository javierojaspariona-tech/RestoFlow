using BiteFlow.Web.Common;
using BiteFlow.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Areas.Catalogo.Controllers;

/// <summary>
/// Ejemplo de referencia: lista el catálogo usando ICatalogoService (cualquier usuario con sesión).
/// Valeria agrega aquí el mantenimiento (Create/Edit/Desactivar) restringido a Administrador.
/// </summary>
[Area("Catalogo")]
public class ProductosController : ModuloControllerBase
{
    private readonly ICatalogoService _catalogo;

    public ProductosController(ICatalogoService catalogo)
    {
        _catalogo = catalogo;
    }

    public async Task<IActionResult> Index(int? idCategoria, CancellationToken ct)
    {
        ViewBag.Categorias = await _catalogo.ListarCategoriasActivasAsync(ct);
        ViewBag.IdCategoria = idCategoria;

        var productos = await _catalogo.ListarProductosDisponiblesAsync(idCategoria, ct);
        return View(productos);
    }
}
