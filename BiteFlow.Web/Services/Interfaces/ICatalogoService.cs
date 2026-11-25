using BiteFlow.Web.Models.Entities;

namespace BiteFlow.Web.Services.Interfaces;

/// <summary>
/// [Catálogo - Valeria] Consultas del catálogo que consumen otros módulos (carrito/pedidos).
/// Solo devuelve categorías y productos ACTIVOS; los productos además deben estar DISPONIBLES.
/// El mantenimiento (crear/editar/desactivar) lo agrega Valeria en su propio servicio.
/// </summary>
public interface ICatalogoService
{
    Task<IReadOnlyList<Categoria>> ListarCategoriasActivasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Producto>> ListarProductosDisponiblesAsync(int? idCategoria = null, CancellationToken ct = default);
    Task<Producto?> ObtenerProductoDisponibleAsync(int idProducto, CancellationToken ct = default);
}
