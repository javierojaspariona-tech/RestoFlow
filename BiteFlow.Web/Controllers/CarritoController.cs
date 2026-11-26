using BiteFlow.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Controllers;
[AllowAnonymous]
public class CarritoController : Controller
{
    private readonly CarritoRepository _carrito; private readonly ProductoRepository _catalogo;
    public CarritoController(CarritoRepository carrito,ProductoRepository catalogo){_carrito=carrito;_catalogo=catalogo;}
    public async Task<IActionResult> Index(CancellationToken ct){var items=_carrito.Obtener();var productos=await _catalogo.ProductosPublicosAsync(ct);foreach(var i in items){var p=productos.FirstOrDefault(x=>x.IdProducto==i.IdProducto);if(p!=null){i.Nombre=p.Nombre;i.Precio=p.Precio;i.ImagenUrl=p.ImagenUrl;}}ViewBag.Total=items.Sum(x=>x.Importe);return View(items);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Agregar(int id,CancellationToken ct){var p=(await _catalogo.ProductosPublicosAsync(ct)).FirstOrDefault(x=>x.IdProducto==id);if(p==null){TempData["Error"]="Producto no disponible.";return RedirectToAction("Index","Home");}var items=_carrito.Obtener();var item=items.FirstOrDefault(x=>x.IdProducto==id);if(item==null)items.Add(new(){IdProducto=id,Nombre=p.Nombre,Precio=p.Precio,Cantidad=1,ImagenUrl=p.ImagenUrl});else item.Cantidad++;HttpContext.Session.SetString("BiteFlow.Carrito",System.Text.Json.JsonSerializer.Serialize(items));TempData["Success"]="Producto agregado al carrito.";return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken] public IActionResult Cantidad(int id,int cantidad){_carrito.CambiarCantidad(id,cantidad);return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken] public IActionResult Quitar(int id){_carrito.Quitar(id);return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken] public IActionResult Vaciar(){_carrito.Vaciar();return RedirectToAction(nameof(Index));}
}
