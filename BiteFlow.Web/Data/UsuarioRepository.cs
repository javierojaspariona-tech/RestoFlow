 using BiteFlow.Web.Models;  using Microsoft.EntityFrameworkCore;
namespace BiteFlow.Web.Data;
public class UsuarioRepository
{
    private readonly AppDbContext _db; public UsuarioRepository(AppDbContext db)=>_db=db;
    public async Task<IReadOnlyList<Usuario>> ListarAsync(CancellationToken ct=default)=>await _db.Usuarios.Include(x=>x.Rol).OrderBy(x=>x.Apellidos).ToListAsync(ct);
    public async Task CambiarEstadoAsync(int id,CancellationToken ct=default){var u=await _db.Usuarios.FindAsync(new object[]{id},ct)??throw new InvalidOperationException("Usuario no encontrado.");u.Activo=!u.Activo;await _db.SaveChangesAsync(ct);}
}
