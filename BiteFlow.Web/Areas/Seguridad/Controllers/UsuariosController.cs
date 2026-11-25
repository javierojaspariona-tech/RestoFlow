using BiteFlow.Web.Common;
using BiteFlow.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Areas.Seguridad.Controllers;

[Area("Seguridad")]
[Authorize(Roles = Roles.Administrador)]
public class UsuariosController : ModuloControllerBase
{
    public IActionResult Index()
    {
        return EnConstruccion("Usuarios", "Javier",
            "Listado de usuarios y roles",
            "Registro, edición y desactivación de usuarios",
            "Registro y administración de clientes (vinculación con usuario)");
    }
}
