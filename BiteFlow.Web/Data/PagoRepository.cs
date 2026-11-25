using System.Data;
using BiteFlow.Web.Common;
using BiteFlow.Web.Models.Dtos;
using BiteFlow.Web.Models.Enums;
using Microsoft.Data.SqlClient;

namespace BiteFlow.Web.Data;

/// <summary>
/// Acceso a datos con ADO.NET y procedimientos almacenados (mismo patrón que ProductoRepository
/// del proyecto de ejemplo del profesor). Se usa donde el procedimiento aporta valor:
///  - sp_registrar_pago: transacción + bloqueo + validaciones en un solo viaje a la BD.
///  - sp_reporte_ventas_paginado: filtro por fechas y paginación (OFFSET/FETCH).
/// </summary>
public class PagoRepository
{
    private readonly string _connectionString;

    public PagoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection' en appsettings.json.");
    }

    public async Task<ResultadoPago> RegistrarPagoSpAsync(
        int idPedido, int idUsuarioCajero, MetodoPago metodoPago, decimal monto, CancellationToken ct = default)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.sp_registrar_pago", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add(new SqlParameter("@IdPedido", SqlDbType.Int) { Value = idPedido });
        cmd.Parameters.Add(new SqlParameter("@IdUsuarioCajero", SqlDbType.Int) { Value = idUsuarioCajero });
        cmd.Parameters.Add(new SqlParameter("@MetodoPago", SqlDbType.VarChar, 20) { Value = metodoPago.ToString() });
        cmd.Parameters.Add(new SqlParameter("@Monto", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = monto });

        var pIdPago = new SqlParameter("@IdPago", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var pNumero = new SqlParameter("@NumeroComprobante", SqlDbType.VarChar, 20) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(pIdPago);
        cmd.Parameters.Add(pNumero);

        try
        {
            await conn.OpenAsync(ct);
            await cmd.ExecuteNonQueryAsync(ct);
        }
        catch (SqlException ex) when (ex.Number >= 50001 && ex.Number <= 50099)
        {
            // Errores de negocio lanzados con THROW dentro del procedimiento (50001..50099)
            throw new DomainException(ex.Message, ex);
        }

        return new ResultadoPago((int)pIdPago.Value, (string)pNumero.Value);
    }

    public async Task<ReporteVentasResultado> BuscarVentasSpAsync(
        DateTime? desde, DateTime? hasta, int pagina, int tamanoPagina, CancellationToken ct = default)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.sp_reporte_ventas_paginado", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add(new SqlParameter("@Desde", SqlDbType.Date) { Value = (object?)desde?.Date ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@Hasta", SqlDbType.Date) { Value = (object?)hasta?.Date ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@Page", SqlDbType.Int) { Value = pagina });
        cmd.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = tamanoPagina });

        await conn.OpenAsync(ct);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var resultado = new ReporteVentasResultado { Pagina = pagina, TamanoPagina = tamanoPagina };

        // 1.er resultado: totales del filtro
        if (await reader.ReadAsync(ct))
        {
            resultado.TotalRegistros = reader.GetInt32(reader.GetOrdinal("TotalRegistros"));
            resultado.TotalVendido = reader.GetDecimal(reader.GetOrdinal("TotalVendido"));
        }

        // 2.º resultado: la página solicitada
        var items = new List<VentaReporteItem>();
        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                items.Add(new VentaReporteItem
                {
                    IdPago = reader.GetInt32(reader.GetOrdinal("IdPago")),
                    NumeroComprobante = reader.GetString(reader.GetOrdinal("NumeroComprobante")),
                    FechaPago = reader.GetDateTime(reader.GetOrdinal("FechaPago")),
                    Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                    MetodoPago = Enum.Parse<MetodoPago>(reader.GetString(reader.GetOrdinal("MetodoPago"))),
                    IdPedido = reader.GetInt32(reader.GetOrdinal("IdPedido")),
                    Cliente = reader.GetString(reader.GetOrdinal("Cliente"))
                });
            }
        }

        resultado.Items = items;
        return resultado;
    }
}
