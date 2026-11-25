using BiteFlow.Web.Models.Entities;

namespace BiteFlow.Web.Services.Interfaces;

/// <summary>
/// [Seguridad - Javier] Consulta de clientes. El CRUD de clientes no está asignado a ningún módulo
/// en el documento del proyecto: se propone que lo asuma Javier porque Cliente se vincula con Usuario.
/// </summary>
public interface IClienteService
{
    Task<Cliente?> ObtenerPorUsuarioAsync(int idUsuario, CancellationToken ct = default);
    Task<Cliente?> ObtenerPorIdAsync(int idCliente, CancellationToken ct = default);
}
