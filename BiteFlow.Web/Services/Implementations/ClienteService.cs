using BiteFlow.Web.Data;
using BiteFlow.Web.Models.Entities;
using BiteFlow.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiteFlow.Web.Services.Implementations;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _db;

    public ClienteService(AppDbContext db)
    {
        _db = db;
    }

    public Task<Cliente?> ObtenerPorUsuarioAsync(int idUsuario, CancellationToken ct = default) =>
        _db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && c.Activo, ct);

    public Task<Cliente?> ObtenerPorIdAsync(int idCliente, CancellationToken ct = default) =>
        _db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.IdCliente == idCliente && c.Activo, ct);
}
