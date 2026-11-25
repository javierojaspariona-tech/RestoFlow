namespace BiteFlow.Web.Models.Enums;

/// <summary>
/// Única fuente de verdad (en C#) de los cambios de estado permitidos.
/// OJO: sp_registrar_pago aplica la misma regla en SQL (Pendiente -> Pagado). Si cambia una, cambia la otra.
///
///   Pedido: Pendiente -> Pagado | Anulado        (Pagado y Anulado son estados finales)
///   Pago:   Registrado -> Anulado                (Anulado es final)
/// </summary>
public static class ReglasDeEstado
{
    private static readonly Dictionary<EstadoPedido, EstadoPedido[]> TransicionesPedido = new()
    {
        [EstadoPedido.Pendiente] = new[] { EstadoPedido.Pagado, EstadoPedido.Anulado },
        [EstadoPedido.Pagado] = Array.Empty<EstadoPedido>(),
        [EstadoPedido.Anulado] = Array.Empty<EstadoPedido>()
    };

    private static readonly Dictionary<EstadoPago, EstadoPago[]> TransicionesPago = new()
    {
        [EstadoPago.Registrado] = new[] { EstadoPago.Anulado },
        [EstadoPago.Anulado] = Array.Empty<EstadoPago>()
    };

    public static bool EsTransicionValida(EstadoPedido actual, EstadoPedido nuevo) =>
        TransicionesPedido.TryGetValue(actual, out var permitidos) && permitidos.Contains(nuevo);

    public static bool EsTransicionValida(EstadoPago actual, EstadoPago nuevo) =>
        TransicionesPago.TryGetValue(actual, out var permitidos) && permitidos.Contains(nuevo);
}
