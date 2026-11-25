namespace BiteFlow.Web.Models.Entities;

public class Producto
{
    public int IdProducto { get; set; }

    public int IdCategoria { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }

    /// <summary>Disponibilidad del día (por ejemplo, "se acabó"). No hay control de stock.</summary>
    public bool Disponible { get; set; } = true;

    /// <summary>Eliminación lógica: un producto inactivo no se puede vender ni se muestra.</summary>
    public bool Activo { get; set; } = true;
}
