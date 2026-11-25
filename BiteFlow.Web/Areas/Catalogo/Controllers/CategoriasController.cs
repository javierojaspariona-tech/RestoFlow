using BiteFlow.Web.Common;
using BiteFlow.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Areas.Catalogo.Controllers;

[Area("Catalogo")]
[Authorize(Roles = Roles.Administrador)]
public class CategoriasController : ModuloControllerBase
{
    public IActionResult Index()
    {
        return EnConstruccion("Categorías", "Valeria",
            "Listado de categorías",
            "Registro y edición de categorías",
            "Desactivación (eliminación lógica)");
    }
}
