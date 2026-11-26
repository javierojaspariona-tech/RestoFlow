
using BiteFlow.Web.Models;

using Microsoft.EntityFrameworkCore;
namespace BiteFlow.Web.Data;
public class ProductoRepository
{
    private readonly AppDbContext _db; public ProductoRepository(AppDbContext db)=>_db=db;
    public async Task<IReadOnlyList<Categoria>> CategoriasPublicasAsync(CancellationToken ct=default)=>await _db.Categorias.Where(x=>x.Activo).OrderBy(x=>x.Nombre).ToListAsync(ct);
    public async Task<IReadOnlyList<Producto>> ProductosPublicosAsync(CancellationToken ct=default)=>await _db.Productos.Include(x=>x.Categoria).Where(x=>x.Activo&&x.Disponible&&x.Categoria!.Activo).OrderBy(x=>x.Nombre).ToListAsync(ct);
    public async Task<IReadOnlyList<Producto>> ProductosAsync(CancellationToken ct=default)=>await _db.Productos.Include(x=>x.Categoria).OrderBy(x=>x.Nombre).ToListAsync(ct);
    public async Task GuardarProductoAsync(Producto p,CancellationToken ct=default){ if(p.IdProducto==0){p.FechaCreacion=DateTime.Now;_db.Productos.Add(p);}else{var old=await _db.Productos.FindAsync(new object[]{p.IdProducto},ct)??throw new InvalidOperationException("Producto no encontrado.");old.Nombre=p.Nombre;old.Descripcion=p.Descripcion;old.Precio=p.Precio;old.IdCategoria=p.IdCategoria;old.ImagenUrl=p.ImagenUrl;old.Disponible=p.Disponible;} await _db.SaveChangesAsync(ct);}
    public async Task EliminarProductoAsync(int id,CancellationToken ct=default){var p=await _db.Productos.FindAsync(new object[]{id},ct)??throw new InvalidOperationException("Producto no encontrado.");p.Activo=false;await _db.SaveChangesAsync(ct);}
    public async Task<IReadOnlyList<Categoria>> CategoriasAsync(CancellationToken ct=default)=>await _db.Categorias.OrderBy(x=>x.Nombre).ToListAsync(ct);
    public async Task GuardarCategoriaAsync(Categoria c,CancellationToken ct=default){if(c.IdCategoria==0)_db.Categorias.Add(c);else{var old=await _db.Categorias.FindAsync(new object[]{c.IdCategoria},ct)??throw new InvalidOperationException("Categoría no encontrada.");old.Nombre=c.Nombre;old.Descripcion=c.Descripcion;}await _db.SaveChangesAsync(ct);}
    public async Task EliminarCategoriaAsync(int id,CancellationToken ct=default){var c=await _db.Categorias.FindAsync(new object[]{id},ct)??throw new InvalidOperationException("Categoría no encontrada.");c.Activo=false;await _db.SaveChangesAsync(ct);}
}
