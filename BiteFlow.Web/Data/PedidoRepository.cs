
using BiteFlow.Web.Models;

using Microsoft.EntityFrameworkCore;
namespace BiteFlow.Web.Data;
public class PedidoRepository
{
    private readonly AppDbContext _db; public PedidoRepository(AppDbContext db)=>_db=db;
    public async Task<long> ConfirmarAsync(int clienteId,string modalidad,byte? mesa,string? observacion,IReadOnlyList<CarritoItem> carrito,CancellationToken ct=default)
    {
        if(carrito.Count==0)throw new InvalidOperationException("El carrito está vacío.");
        if(modalidad=="Mesa" && (mesa is null || mesa<1 || mesa>99))throw new InvalidOperationException("Selecciona una mesa válida.");
        if(modalidad=="ParaLlevar")mesa=null; else if(modalidad!="Mesa")throw new InvalidOperationException("Modalidad no válida.");
        await using var tx=await _db.Database.BeginTransactionAsync(ct);
        var ids=carrito.Select(x=>x.IdProducto).Distinct().ToList();
        var productos=await _db.Productos.Where(x=>ids.Contains(x.IdProducto)&&x.Activo&&x.Disponible).ToDictionaryAsync(x=>x.IdProducto,ct);
        if(productos.Count!=ids.Count)throw new InvalidOperationException("Uno o más productos ya no están disponibles.");
        var detalles=new List<DetallePedido>(); decimal total=0;
        foreach(var item in carrito){if(!productos.TryGetValue(item.IdProducto,out var p))throw new InvalidOperationException("Producto no disponible.");var qty=Math.Clamp(item.Cantidad,1,99);detalles.Add(new DetallePedido{IdProducto=p.IdProducto,NombreProducto=p.Nombre,Cantidad=qty,PrecioUnitario=p.Precio});total+=p.Precio*qty;}
        total=Math.Round(total,2); var igv=Math.Round(total*18m/118m,2); var subtotal=total-igv;
        var pedido=new Pedido{IdCliente=clienteId,Codigo=$"BF-{DateTime.Now:yyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..4].ToUpperInvariant()}",Modalidad=modalidad,NumeroMesa=mesa,Estado="Pendiente",Subtotal=subtotal,Igv=igv,Total=total,FechaPedido=DateTime.Now,Observacion=observacion,Detalles=detalles};
        _db.Pedidos.Add(pedido); await _db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return pedido.IdPedido;
    }
    public async Task<IReadOnlyList<Pedido>> MisPedidosAsync(int id,CancellationToken ct=default)=>await _db.Pedidos.Include(x=>x.Detalles).Where(x=>x.IdCliente==id).OrderByDescending(x=>x.FechaPedido).ToListAsync(ct);
    public Task<Pedido?> ObtenerAsync(long id,CancellationToken ct=default)=>_db.Pedidos.Include(x=>x.Detalles).Include(x=>x.Pago).FirstOrDefaultAsync(x=>x.IdPedido==id,ct);
    public async Task AnularAsync(long id,int cliente,CancellationToken ct=default){var p=await _db.Pedidos.FirstOrDefaultAsync(x=>x.IdPedido==id&&x.IdCliente==cliente,ct)??throw new InvalidOperationException("Pedido no encontrado.");if(p.Estado!="Pendiente")throw new InvalidOperationException("Solo se pueden anular pedidos pendientes.");p.Estado="Anulado";await _db.SaveChangesAsync(ct);}
    public async Task<IReadOnlyList<Pedido>> PendientesAsync(CancellationToken ct=default)=>await _db.Pedidos.Include(x=>x.Cliente)!.ThenInclude(x=>x!.Usuario).Where(x=>x.Estado=="Pendiente").OrderBy(x=>x.FechaPedido).ToListAsync(ct);
}
