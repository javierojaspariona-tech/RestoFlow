namespace BiteFlow.Web.Models.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    /// <summary>Hash PBKDF2 (ver IPasswordHasherService). Nunca se guarda la contraseña en texto plano.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public int IdRol { get; set; }
    public Rol Rol { get; set; } = null!;

    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    /// <summary>Perfil de cliente vinculado a este usuario (0..1).</summary>
    public Cliente? Cliente { get; set; }

    public string NombreCompleto => $"{Nombres} {Apellidos}";
}
