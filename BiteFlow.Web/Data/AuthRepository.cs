
using BiteFlow.Web.Models;


using Microsoft.EntityFrameworkCore;
namespace BiteFlow.Web.Data;
public class AuthRepository
{
    private readonly AppDbContext _db; private readonly PasswordHasher _hasher;
    public AuthRepository(AppDbContext db, PasswordHasher hasher) { _db=db; _hasher=hasher; }
    public async Task<Usuario?> ValidarAsync(string correo, string password, CancellationToken ct=default)
    {
        var user = await _db.Usuarios.Include(x=>x.Rol).Include(x=>x.Cliente)
            .FirstOrDefaultAsync(x=>x.Correo==correo && x.Activo, ct);
        if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash) || !_hasher.Verify(password, user.PasswordHash))
            return null;
        return user;
    }
    public async Task RegistrarClienteAsync(string nombres,string apellidos,string correo,string password,string? telefono,string? direccion,CancellationToken ct=default)
    {
        if(await _db.Usuarios.AnyAsync(x=>x.Correo==correo,ct)) throw new InvalidOperationException("El correo ya está registrado.");
        var rol=await _db.Roles.SingleOrDefaultAsync(x=>x.Nombre==Roles.Cliente,ct) ?? throw new InvalidOperationException("Ejecuta BiteFlow.Web/Data/Sp/02_seed.sql antes de registrar clientes.");
        await using var tx=await _db.Database.BeginTransactionAsync(ct);
        var user=new Usuario { IdRol=rol.IdRol,Nombres=nombres,Apellidos=apellidos,Correo=correo,PasswordHash=_hasher.Hash(password),Activo=true,FechaCreacion=DateTime.Now };
        _db.Usuarios.Add(user); await _db.SaveChangesAsync(ct);
        _db.Clientes.Add(new Cliente { IdUsuario=user.IdUsuario,Telefono=telefono,Direccion=direccion });
        await _db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
    }
}
