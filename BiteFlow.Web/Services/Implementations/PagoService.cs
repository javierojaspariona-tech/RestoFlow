using BiteFlow.Web.Common;
using BiteFlow.Web.Data;
using BiteFlow.Web.Models.Dtos;
using BiteFlow.Web.Models.Enums;
using BiteFlow.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiteFlow.Web.Services.Implementations;

public class PagoService : IPagoService
{
    private readonly AppDbContext _db;
    private readonly PagoRepository _pagoRepository;
    private readonly ICurrentUserService _usuarioActual;

    public PagoService(AppDbContext db, PagoRepository pagoRepository, ICurrentUserService usuarioActual)
    {
        _db = db;
        _pagoRepository = pagoRepository;
        _usuarioActual = usuarioActual;
    }

    public async Task<ResultadoPago> RegistrarPagoAsync(int idPedido, MetodoPago metodoPago, CancellationToken ct = default)
    {
        // La autorización por rol (Cajero/Administrador) se exige en el controlador con [Authorize].
        var idCajero = _usuarioActual.IdUsuario
            ?? throw new DomainException("No se pudo identificar al cajero con la sesión iniciada.");

        if (!Enum.IsDefined(metodoPago))
            throw new DomainException("El método de pago no es válido.");

        // El monto SIEMPRE sale de la BD (total del pedido), nunca del formulario.
        var total = await _db.Pedidos
            .AsNoTracking()
            .Where(p => p.IdPedido == idPedido)
            .Select(p => (decimal?)p.Total)
            .FirstOrDefaultAsync(ct)
            ?? throw new DomainException("El pedido no existe.");

        // Transacción, bloqueo y validaciones (estado Pendiente, pago único, monto) dentro del SP.
        return await _pagoRepository.RegistrarPagoSpAsync(idPedido, idCajero, metodoPago, total, ct);
    }
}
