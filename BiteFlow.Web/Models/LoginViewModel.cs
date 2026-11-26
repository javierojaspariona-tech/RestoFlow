using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class LoginViewModel
{
    [Required, EmailAddress] public string Correo { get; set; } = "";
    [Required, DataType(DataType.Password)] public string Password { get; set; } = "";
    public bool Recordarme { get; set; }
}
