using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class RegistroClienteViewModel
{
    [Required, StringLength(100)] public string Nombres { get; set; } = "";
    [Required, StringLength(100)] public string Apellidos { get; set; } = "";
    [Required, EmailAddress] public string Correo { get; set; } = "";
    [Required, MinLength(8), DataType(DataType.Password)] public string Password { get; set; } = "";
    [Phone] public string? Telefono { get; set; } public string? Direccion { get; set; }
}
