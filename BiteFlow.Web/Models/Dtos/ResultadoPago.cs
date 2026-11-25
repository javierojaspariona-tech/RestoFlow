namespace BiteFlow.Web.Models.Dtos;

/// <summary>Resultado de registrar un pago: datos para generar el comprobante interno.</summary>
public record ResultadoPago(int IdPago, string NumeroComprobante);
