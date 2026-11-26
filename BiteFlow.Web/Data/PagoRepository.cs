using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BiteFlow.Web.Models;
namespace BiteFlow.Web.Data;
public class PagoRepository
{
    private readonly string _connectionString;
    public PagoRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    public async Task<ResultadoPago> RegistrarPagoAsync(long idPedido, int idUsuarioCajero, string metodoPago, decimal monto, CancellationToken ct = default)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.sp_registrar_pago", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@IdPedido", SqlDbType.BigInt).Value = idPedido;
        cmd.Parameters.Add("@IdUsuarioCajero", SqlDbType.Int).Value = idUsuarioCajero;
        cmd.Parameters.Add("@MetodoPago", SqlDbType.NVarChar, 20).Value = metodoPago;
        var pm = cmd.Parameters.Add("@Monto", SqlDbType.Decimal); pm.Precision = 12; pm.Scale = 2; pm.Value = monto;
        await cn.OpenAsync(ct);
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        if (!await rd.ReadAsync(ct)) throw new InvalidOperationException("No se recibió el comprobante del procedimiento de pago.");
        return new ResultadoPago { IdPago=rd.GetInt64(rd.GetOrdinal("IdPago")), IdPedido=rd.GetInt64(rd.GetOrdinal("IdPedido")), Monto=rd.GetDecimal(rd.GetOrdinal("Monto")), MetodoPago=rd.GetString(rd.GetOrdinal("MetodoPago")), NumeroComprobante=rd.GetString(rd.GetOrdinal("NumeroComprobante")), FechaPago=rd.GetDateTime(rd.GetOrdinal("FechaPago")) };
    }
    public async Task<PaginaResultado<ReporteVentaDto>> ReporteAsync(DateTime inicio, DateTime fin, int pagina, int tamano, CancellationToken ct = default)
    {
        var result = new PaginaResultado<ReporteVentaDto> { Pagina=pagina, TamanoPagina=tamano };
        var list = new List<ReporteVentaDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.sp_reporte_ventas_paginado", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = inicio.Date;
        cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = fin.Date;
        cmd.Parameters.Add("@Pagina", SqlDbType.Int).Value = pagina;
        cmd.Parameters.Add("@TamanoPagina", SqlDbType.Int).Value = tamano;
        await cn.OpenAsync(ct);
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        while (await rd.ReadAsync(ct))
        {
            var row = new ReporteVentaDto { IdPago=rd.GetInt64(rd.GetOrdinal("IdPago")), CodigoPedido=rd.GetString(rd.GetOrdinal("CodigoPedido")), NumeroComprobante=rd.GetString(rd.GetOrdinal("NumeroComprobante")), FechaPago=rd.GetDateTime(rd.GetOrdinal("FechaPago")), MetodoPago=rd.GetString(rd.GetOrdinal("MetodoPago")), Monto=rd.GetDecimal(rd.GetOrdinal("Monto")), Cajero=rd.GetString(rd.GetOrdinal("Cajero")), TotalRegistros=rd.GetInt32(rd.GetOrdinal("TotalRegistros")) };
            list.Add(row);
        }
        result.Items = list;
        result.TotalRegistros = list.FirstOrDefault()?.TotalRegistros ?? 0;
        return result;
    }
}
