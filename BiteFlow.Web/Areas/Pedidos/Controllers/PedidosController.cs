using BiteFlow.Web.Common;
using BiteFlow.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Areas.Pedidos.Controllers;

[Area("Pedidos")]
[Authorize(Roles = Roles.Cliente)]
public class PedidosController : ModuloControllerBase
{
    public IActionResult Index()
    {
        return EnConstruccion("Mis pedidos", "Raúl",
            "Listado de pedidos del cliente (IPedidoService.ListarPorClienteAsync)",
            "Detalle de un pedido (IPedidoService.ObtenerPedidoAsync)",
            "Anular un pedido pendiente (IPedidoService.AnularPedidoAsync)");
    }
}
