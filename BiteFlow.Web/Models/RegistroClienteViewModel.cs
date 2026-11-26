using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class RegistroClienteViewModel
{
    [Required(ErrorMessage = "Ingresa tus nombres."), StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Nombres { get; set; } = "";
    [Required(ErrorMessage = "Ingresa tus apellidos."), StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Apellidos { get; set; } = "";
    [Required(ErrorMessage = "Ingresa tu correo."), EmailAddress(ErrorMessage = "Ingresa un correo válido."), Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = "";
    [Required(ErrorMessage = "Ingresa una contraseña."), MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres."), DataType(DataType.Password), Display(Name = "Contraseña")]
    public string Password { get; set; } = "";
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden."), DataType(DataType.Password), Display(Name = "Confirmar contraseña")]
    public string? ConfirmarPassword { get; set; }
    [Phone(ErrorMessage = "Ingresa un teléfono válido."), StringLength(20, ErrorMessage = "Máximo 20 caracteres."), Display(Name = "Teléfono")] public string? Telefono { get; set; }
    [Display(Name = "Dirección"), StringLength(250, ErrorMessage = "Máximo 250 caracteres.")] public string? Direccion { get; set; }
}
