
namespace BiteFlow.Web.Models
{
    public class EnConstruccionViewModel
    {
        public string Modulo { get; set; } = string.Empty;

        public string Responsable { get; set; } = string.Empty;

        public List<string> Pendientes { get; set; } = new List<string>();
    }
}
