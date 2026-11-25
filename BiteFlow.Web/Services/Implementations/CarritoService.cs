using System.Text.Json;
using BiteFlow.Web.Common;
using BiteFlow.Web.Models.Dtos;
using BiteFlow.Web.Models.Entities;
using BiteFlow.Web.Services.Interfaces;

namespace BiteFlow.Web.Services.Implementations;

/// <summary>Implementación base del carrito en sesión (JSON). Raúl puede extenderla.</summary>
public class CarritoService : ICarritoService
{
    private const string ClaveSesion = "biteflow.carrito";
    private readonly IHttpContextAccessor _accessor;

    public CarritoService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private ISession Sesion =>
        _accessor.HttpContext?.Session ?? throw new InvalidOperationException("No hay una sesión HTTP disponible.");

    private List<CarritoItem> Leer()
    {
        var json = Sesion.GetString(ClaveSesion);
        if (string.IsNullOrWhiteSpace(json))
            return new List<CarritoItem>();

        return JsonSerializer.Deserialize<List<CarritoItem>>(json) ?? new List<CarritoItem>();
    }

    private void Guardar(List<CarritoItem> items)
    {
        Sesion.SetString(ClaveSesion, JsonSerializer.Serialize(items));
    }

    public IReadOnlyList<CarritoItem> Obtener() => Leer();

    public decimal ObtenerTotal() => Leer().Sum(i => i.Subtotal);

    public void Agregar(Producto producto, int cantidad = 1)
    {
        if (cantidad <= 0)
            throw new DomainException("La cantidad debe ser mayor que cero.");
        if (!producto.Activo || !producto.Disponible)
            throw new DomainException($"El producto '{producto.Nombre}' no está disponible.");

        var items = Leer();
        var existente = items.FirstOrDefault(i => i.IdProducto == producto.IdProducto);
        if (existente is null)
        {
            items.Add(new CarritoItem
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                PrecioUnitario = producto.Precio,
                Cantidad = cantidad
            });
        }
        else
        {
            existente.Cantidad += cantidad;
            existente.PrecioUnitario = producto.Precio;
        }

        Guardar(items);
    }

    public void CambiarCantidad(int idProducto, int cantidad)
    {
        if (cantidad <= 0)
        {
            Quitar(idProducto);
            return;
        }

        var items = Leer();
        var existente = items.FirstOrDefault(i => i.IdProducto == idProducto);
        if (existente is null)
            return;

        existente.Cantidad = cantidad;
        Guardar(items);
    }

    public void Quitar(int idProducto)
    {
        var items = Leer();
        items.RemoveAll(i => i.IdProducto == idProducto);
        Guardar(items);
    }

    public void Vaciar()
    {
        Sesion.Remove(ClaveSesion);
    }
}
