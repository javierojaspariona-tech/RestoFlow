namespace BiteFlow.Web.Models;
public class PaginaResultado<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int Pagina { get; set; } public int TamanoPagina { get; set; }
    public int TotalRegistros { get; set; }
    public int TotalPaginas => (int)Math.Ceiling(TotalRegistros / (double)TamanoPagina);
}
