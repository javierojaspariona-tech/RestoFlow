namespace BiteFlow.Web.Models.ViewModels;

/// <summary>Modelo de la pantalla "En construcción" (Views/Shared/EnConstruccion.cshtml).</summary>
public record EnConstruccionViewModel(string Modulo, string Responsable, IReadOnlyList<string> Pendientes);
