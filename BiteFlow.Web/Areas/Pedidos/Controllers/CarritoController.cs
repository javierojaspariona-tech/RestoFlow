using BiteFlow.Web.Common;
using BiteFlow.Web.Security;
using BiteFlow.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Areas.Pedidos.Controllers;

/// <summary>
/// Ejemplo de referencia: muestra el carrito guardado en sesión (ICarritoService).
/// Raúl agrega: Agregar, CambiarCantidad, Quitar y Confirmar (IPedidoService.ConfirmarPedidoAsync).
/// </summary>
[Area("Pedidos")]
[Authorize(Roles = Roles.Cliente)]
public class CarritoController : ModuloControllerBase
{
    private readonly ICarritoService _carrito;

    public CarritoController(ICarritoService carrito)
    {
        _carrito = carrito;
    }

    public IActionResult Index()
    {
        ViewBag.Total = _carrito.ObtenerTotal();
        return View(_carrito.Obtener());
    }
}
