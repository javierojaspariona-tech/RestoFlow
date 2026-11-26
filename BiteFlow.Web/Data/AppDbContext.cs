using BiteFlow.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BiteFlow.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Rol");
            entity.HasKey(x => x.IdRol);
            entity.Property(x => x.Nombre).HasMaxLength(30).IsRequired();
            entity.HasIndex(x => x.Nombre).IsUnique();
            entity.HasMany(x => x.Usuarios).WithOne(x => x.Rol)
                .HasForeignKey(x => x.IdRol).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(x => x.IdUsuario);
            entity.Property(x => x.Nombres).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Apellidos).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Correo).HasMaxLength(254).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.HasIndex(x => x.Correo).IsUnique();
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");
            entity.HasKey(x => x.IdCliente);
            entity.Property(x => x.Telefono).HasMaxLength(20);
            entity.Property(x => x.Direccion).HasMaxLength(250);
            entity.HasOne(x => x.Usuario).WithOne(x => x.Cliente)
                .HasForeignKey<Cliente>(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.IdUsuario).IsUnique();
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categoria");
            entity.HasKey(x => x.IdCategoria);
            entity.Property(x => x.Nombre).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(250);
            entity.HasIndex(x => x.Nombre).IsUnique();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Producto");
            entity.HasKey(x => x.IdProducto);
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(500);
            entity.Property(x => x.ImagenUrl).HasMaxLength(500);
            entity.Property(x => x.Precio).HasColumnType("decimal(10,2)");
            entity.HasOne(x => x.Categoria).WithMany(x => x.Productos)
                .HasForeignKey(x => x.IdCategoria).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.IdCategoria, x.Activo, x.Disponible });
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("Pedido");
            entity.HasKey(x => x.IdPedido);
            entity.Property(x => x.Codigo).HasMaxLength(24).IsRequired();
            entity.Property(x => x.Modalidad).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Estado).HasMaxLength(15).IsRequired();
            entity.Property(x => x.Subtotal).HasColumnType("decimal(12,2)");
            entity.Property(x => x.Igv).HasColumnType("decimal(12,2)");
            entity.Property(x => x.Total).HasColumnType("decimal(12,2)");
            entity.Property(x => x.Observacion).HasMaxLength(300);
            entity.HasIndex(x => x.Codigo).IsUnique();
            entity.HasOne(x => x.Cliente).WithMany(x => x.Pedidos)
                .HasForeignKey(x => x.IdCliente).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.ToTable("DetallePedido");
            entity.HasKey(x => x.IdDetallePedido);
            entity.Property(x => x.NombreProducto).HasMaxLength(120).IsRequired();
            entity.Property(x => x.PrecioUnitario).HasColumnType("decimal(10,2)");
            entity.Property(x => x.Importe)
                .HasColumnType("decimal(12,2)")
                .HasComputedColumnSql("CONVERT(DECIMAL(12,2), Cantidad * PrecioUnitario)", stored: true);
            entity.HasOne(x => x.Pedido).WithMany(x => x.Detalles)
                .HasForeignKey(x => x.IdPedido).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Producto).WithMany(x => x.Detalles)
                .HasForeignKey(x => x.IdProducto).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.ToTable("Pago");
            entity.HasKey(x => x.IdPago);
            entity.Property(x => x.MetodoPago).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Monto).HasColumnType("decimal(12,2)");
            entity.Property(x => x.Estado).HasMaxLength(15).IsRequired();
            entity.Property(x => x.NumeroComprobante).HasMaxLength(24).IsRequired();
            entity.HasIndex(x => x.IdPedido).IsUnique();
            entity.HasIndex(x => x.NumeroComprobante).IsUnique();
            entity.HasOne(x => x.Pedido).WithOne(x => x.Pago)
                .HasForeignKey<Pago>(x => x.IdPedido).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.UsuarioCajero).WithMany(x => x.Pagos)
                .HasForeignKey(x => x.IdUsuarioCajero).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
