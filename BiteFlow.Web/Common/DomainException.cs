namespace BiteFlow.Web.Common;

/// <summary>
/// Error de regla de negocio (carrito vacío, pago duplicado, cambio de estado no permitido, etc.).
/// Los controladores deben capturarla y mostrar el mensaje al usuario (TempData["Error"]).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
