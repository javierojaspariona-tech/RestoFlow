namespace BiteFlow.Web.Services.Interfaces;

/// <summary>[Seguridad - Javier] Datos del usuario que tiene la sesión iniciada.</summary>
public interface ICurrentUserService
{
    bool EstaAutenticado { get; }
    int? IdUsuario { get; }
    string? Correo { get; }
    string? Rol { get; }
}
