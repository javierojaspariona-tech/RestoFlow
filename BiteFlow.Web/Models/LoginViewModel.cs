using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class LoginViewModel
{
    [Required(ErrorMessage = "Ingresa tu correo."), EmailAddress(ErrorMessage = "Ingresa un correo válido."), Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = "";
    [Required(ErrorMessage = "Ingresa tu contraseña."), DataType(DataType.Password), Display(Name = "Contraseña")]
    public string Password { get; set; } = "";
    public bool Recordarme { get; set; }
}
