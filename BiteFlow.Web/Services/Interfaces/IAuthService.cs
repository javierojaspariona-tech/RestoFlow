using BiteFlow.Web.Models.Entities;

namespace BiteFlow.Web.Services.Interfaces;

/// <summary>[Seguridad - Javier] Validación de credenciales para el inicio de sesión.</summary>
public interface IAuthService
{
    /// <summary>Devuelve el usuario (con su Rol) si las credenciales son válidas y está activo; si no, null.</summary>
    Task<Usuario?> ValidarCredencialesAsync(string correo, string password, CancellationToken ct = default);
}
