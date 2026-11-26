using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class CategoriaViewModel { public int IdCategoria { get; set; } [Required, StringLength(80)] public string Nombre { get; set; } = ""; [StringLength(250)] public string? Descripcion { get; set; } }
