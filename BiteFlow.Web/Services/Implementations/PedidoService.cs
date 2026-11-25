using BiteFlow.Web.Common;
using BiteFlow.Web.Data;
using BiteFlow.Web.Models.Dtos;
using BiteFlow.Web.Models.Entities;
using BiteFlow.Web.Models.Enums;
using BiteFlow.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiteFlow.Web.Services.Implementations;

public class PedidoService : IPedidoService
{
    private readonly AppDbContext _db;

    public PedidoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> ConfirmarPedidoAsync(int idCliente, IReadOnlyList<CarritoItem> items, CancellationToken ct = default)
    {
        if (items is null || items.Count == 0)
            throw new DomainException("El carrito está vacío.");
        if (items.Any(i => i.Cantidad <= 0))
            throw new DomainException("Todas las cantidades deben ser mayores que cero.");

        // Todo o nada: si algo falla antes del Commit, al salir del bloque se hace Rollback automático.
        await using var transaccion = await _db.Database.BeginTransactionAsync(ct);

        var clienteActivo = await _db.Clientes.AnyAsync(c => c.IdCliente == idCliente && c.Activo, ct);
        if (!clienteActivo)
            throw new DomainException("El cliente no existe o está inactivo.");

        // Los precios y la disponibilidad se leen de la BD; nunca se confía en lo que trae el carrito.
        var idsProductos = items.Select(i => i.IdProducto).Distinct().ToList();
        var productos = await _db.Productos
            .Where(p => idsProductos.Contains(p.IdProducto))
            .ToDictionaryAsync(p => p.IdProducto, ct);

        var pedido = new Pedido { IdCliente = idCliente, FechaPedido = DateTime.Now };

        foreach (var grupo in items.GroupBy(i => i.IdProducto))
        {
            if (!productos.TryGetValue(grupo.Key, out var producto) || !producto.Activo || !producto.Disponible)
                throw new DomainException($"El producto con código {grupo.Key} ya no está disponible.");

            // Guarda el precio unitario histórico en el detalle
            pedido.AgregarDetalle(producto, grupo.Sum(i => i.Cantidad));
        }

        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync(ct);
        await transaccion.CommitAsync(ct);

        return pedido.IdPedido;
    }

    public Task<Pedido?> ObtenerPedidoAsync(int idPedido, CancellationToken ct = default)
    {
        return _db.Pedidos
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Detalles).ThenInclude(d => d.Producto)
            .Include(p => p.Pago)
            .FirstOrDefaultAsync(p => p.IdPedido == idPedido, ct);
    }

    public async Task<IReadOnlyList<Pedido>> ListarPendientesDePagoAsync(CancellationToken ct = default)
    {
        return await _db.Pedidos
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Where(p => p.Estado == EstadoPedido.Pendiente)
            .OrderBy(p => p.FechaPedido)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Pedido>> ListarPorClienteAsync(int idCliente, CancellationToken ct = default)
    {
        return await _db.Pedidos
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Where(p => p.IdCliente == idCliente)
            .OrderByDescending(p => p.FechaPedido)
            .ToListAsync(ct);
    }

    public async Task AnularPedidoAsync(int idPedido, CancellationToken ct = default)
    {
        var pedido = await _db.Pedidos
            .Include(p => p.Pago)
            .FirstOrDefaultAsync(p => p.IdPedido == idPedido, ct)
            ?? throw new DomainException("El pedido no existe.");

        if (pedido.Pago is not null)
            throw new DomainException("No se puede anular un pedido que ya tiene un pago registrado.");

        // Lanza DomainException si la transición no está permitida (p. ej. Pagado -> Anulado)
        pedido.CambiarEstado(EstadoPedido.Anulado);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainException("El pedido fue modificado por otra operación. Actualiza la pantalla e inténtalo de nuevo.");
        }
    }
}
