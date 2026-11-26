using BiteFlow.Web.Data;
using BiteFlow.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BiteFlow.Web.Controllers;
public class HomeController : Controller
{
    private readonly ProductoRepository _catalogo;
    public HomeController(ProductoRepository catalogo)=>_catalogo=catalogo;
    [AllowAnonymous] public async Task<IActionResult> Index(CancellationToken ct) => View(await _catalogo.ProductosPublicosAsync(ct));
    [Authorize(Roles=Roles.Administrador)] public IActionResult Admin()=>View();
    [ResponseCache(Duration=0,Location=ResponseCacheLocation.None,NoStore=true)]
    public IActionResult Error()=>View(new ErrorViewModel{RequestId=Activity.Current?.Id??HttpContext.TraceIdentifier});
}
