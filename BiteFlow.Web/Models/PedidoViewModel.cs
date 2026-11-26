using System.ComponentModel.DataAnnotations;
namespace BiteFlow.Web.Models;
public class PedidoViewModel
{
    [Required] public string Modalidad { get; set; } = "ParaLlevar";
    [Range(1,99)] public byte? NumeroMesa { get; set; }
    [StringLength(300)] public string? Observacion { get; set; }
}
