namespace BiteFlow.Web.Models.Enums;

/// <summary>Métodos de pago aceptados en caja. En la BD se guardan como texto.</summary>
public enum MetodoPago
{
    Efectivo = 1,
    Yape = 2,
    Plin = 3,
    Tarjeta = 4
}
