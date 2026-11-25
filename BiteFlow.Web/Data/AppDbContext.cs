using BiteFlow.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BiteFlow.Web.Data;

/// <summary>
/// DbContext único y compartido. La configuración de cada entidad está en Data/Configurations
/// (una clase por entidad) para evitar conflictos de Git entre los integrantes.
/// El esquema real lo crea Database/01_schema.sql: si cambias una configuración, cambia también el script.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
