using BiteFlow.Web.Data;
using BiteFlow.Web.Models.Entities;
using BiteFlow.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiteFlow.Web.Services.Implementations;

public class CatalogoService : ICatalogoService
{
    private readonly AppDbContext _db;

    public CatalogoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Categoria>> ListarCategoriasActivasAsync(CancellationToken ct = default)
    {
        return await _db.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Producto>> ListarProductosDisponiblesAsync(int? idCategoria = null, CancellationToken ct = default)
    {
        return await _db.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Where(p => p.Activo && p.Disponible && p.Categoria.Activo
                        && (idCategoria == null || p.IdCategoria == idCategoria))
            .OrderBy(p => p.Categoria.Nombre)
            .ThenBy(p => p.Nombre)
            .ToListAsync(ct);
    }

    public Task<Producto?> ObtenerProductoDisponibleAsync(int idProducto, CancellationToken ct = default)
    {
        return _db.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.IdProducto == idProducto && p.Activo && p.Disponible && p.Categoria.Activo, ct);
    }
}
