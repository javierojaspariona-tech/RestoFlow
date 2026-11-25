namespace BiteFlow.Web.Security;

/// <summary>
/// Nombres de roles. Deben coincidir EXACTAMENTE con la columna Rol.Nombre de la BD.
/// Uso: [Authorize(Roles = Roles.Administrador)]  o  [Authorize(Roles = Roles.AdministradorOCajero)]
/// </summary>
public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Cajero = "Cajero";
    public const string Cliente = "Cliente";

    public const string AdministradorOCajero = Administrador + "," + Cajero;
}
