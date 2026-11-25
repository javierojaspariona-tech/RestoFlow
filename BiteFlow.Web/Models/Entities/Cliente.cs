namespace BiteFlow.Web.Models.Entities;

public class Cliente
{
    public int IdCliente { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>DNI de 8 dígitos. Único: evita registrar dos veces a la misma persona.</summary>
    public string Dni { get; set; } = string.Empty;

    public string? Telefono { get; set; }
    public string? Correo { get; set; }

    /// <summary>
    /// Usuario vinculado (opcional). La relación es 1 a 0..1 y el índice único filtrado
    /// UX_Cliente_IdUsuario impide que dos clientes apunten al mismo usuario.
    /// </summary>
    public int? IdUsuario { get; set; }
    public Usuario? Usuario { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public string NombreCompleto => $"{Nombres} {Apellidos}";
}
