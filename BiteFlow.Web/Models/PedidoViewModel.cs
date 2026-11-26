using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class PedidoViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Selecciona la modalidad.")] public string Modalidad { get; set; } = "ParaLlevar";
    [Display(Name = "Número de mesa"), Range(1, 99, ErrorMessage = "La mesa debe estar entre 1 y 99.")] public byte? NumeroMesa { get; set; }
    [Display(Name = "Observación"), StringLength(300, ErrorMessage = "Máximo 300 caracteres.")] public string? Observacion { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Modalidad != "ParaLlevar" && Modalidad != "Mesa")
            yield return new ValidationResult("Modalidad no válida.", new[] { nameof(Modalidad) });
        if (Modalidad == "Mesa" && NumeroMesa is null)
            yield return new ValidationResult("Indica el número de mesa.", new[] { nameof(NumeroMesa) });
    }
}
