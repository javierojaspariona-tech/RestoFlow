using BiteFlow.Web.Models.Dtos;
using BiteFlow.Web.Models.Entities;

namespace BiteFlow.Web.Services.Interfaces;

/// <summary>[Pedidos - Raúl] Carrito de compras guardado en sesión (JSON). No usa la base de datos.</summary>
public interface ICarritoService
{
    IReadOnlyList<CarritoItem> Obtener();
    decimal ObtenerTotal();
    void Agregar(Producto producto, int cantidad = 1);

    /// <summary>Cambia la cantidad de una línea. Si la cantidad es 0 o menor, la línea se elimina.</summary>
    void CambiarCantidad(int idProducto, int cantidad);
    void Quitar(int idProducto);
    void Vaciar();
}
