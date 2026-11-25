using BiteFlow.Web.Data;
using BiteFlow.Web.Models.Entities;
using BiteFlow.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiteFlow.Web.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasherService _hasher;

    public AuthService(AppDbContext db, IPasswordHasherService hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<Usuario?> ValidarCredencialesAsync(string correo, string password, CancellationToken ct = default)
    {
        var correoNormalizado = correo.Trim();

        var usuario = await _db.Usuarios
            .AsNoTracking()
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Correo == correoNormalizado && u.Activo, ct);

        if (usuario is null)
            return null;

        return _hasher.Verify(password, usuario.PasswordHash) ? usuario : null;
    }
}
