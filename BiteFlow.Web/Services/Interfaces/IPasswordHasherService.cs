namespace BiteFlow.Web.Services.Interfaces;

/// <summary>[Seguridad - Javier] Hash y verificación de contraseñas (PBKDF2). Nunca guardar texto plano.</summary>
public interface IPasswordHasherService
{
    string Hash(string password);
    bool Verify(string password, string hashAlmacenado);
}
