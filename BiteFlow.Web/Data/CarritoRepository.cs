using System.Text.Json;
using BiteFlow.Web.Models;
using Microsoft.AspNetCore.Http;
namespace BiteFlow.Web.Data;
public class CarritoRepository
{
    private const string Key="BiteFlow.Carrito"; private readonly IHttpContextAccessor _http;
    public CarritoRepository(IHttpContextAccessor http)=>_http=http;
    public List<CarritoItem> Obtener()=>JsonSerializer.Deserialize<List<CarritoItem>>(_http.HttpContext!.Session.GetString(Key)??"[]")??new();
    private void Guardar(List<CarritoItem> items)=>_http.HttpContext!.Session.SetString(Key,JsonSerializer.Serialize(items));
    public void Agregar(int id){var items=Obtener();var item=items.FirstOrDefault(x=>x.IdProducto==id);if(item==null)throw new InvalidOperationException("Producto no encontrado en el carrito.");item.Cantidad++;Guardar(items);}
    public void CambiarCantidad(int id,int cantidad){var items=Obtener();var item=items.FirstOrDefault(x=>x.IdProducto==id);if(item==null)return;if(cantidad<=0)items.Remove(item);else item.Cantidad=Math.Min(cantidad,99);Guardar(items);}
    public void Quitar(int id){var items=Obtener();items.RemoveAll(x=>x.IdProducto==id);Guardar(items);}
    public void Vaciar()=>Guardar(new());
}
