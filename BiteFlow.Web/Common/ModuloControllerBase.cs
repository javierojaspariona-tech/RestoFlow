using BiteFlow.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Common;

/// <summary>
/// Base para los controladores de las Areas. Incluye el helper para las pantallas
/// que todavía no se desarrollan ("En construcción").
/// </summary>
public abstract class ModuloControllerBase : Controller
{
    protected IActionResult EnConstruccion(string modulo, string responsable, params string[] pendientes)
    {
        return View("~/Views/Shared/EnConstruccion.cshtml",
            new EnConstruccionViewModel(modulo, responsable, pendientes));
    }
}
