using System.Security.Claims;
using BiteFlow.Web.Services.Interfaces;

namespace BiteFlow.Web.Services.Implementations;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private ClaimsPrincipal? Usuario => _accessor.HttpContext?.User;

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated ?? false;

    public int? IdUsuario =>
        int.TryParse(Usuario?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : (int?)null;

    public string? Correo => Usuario?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Rol => Usuario?.FindFirst(ClaimTypes.Role)?.Value;
}
