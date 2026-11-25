using BiteFlow.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteFlow.Web.Data.Configurations;

public class DetallePedidoConfiguration : IEntityTypeConfiguration<DetallePedido>
{
    public void Configure(EntityTypeBuilder<DetallePedido> builder)
    {
        builder.ToTable("DetallePedido");
        builder.HasKey(d => d.IdDetalle);
        builder.Property(d => d.PrecioUnitario).HasColumnType("decimal(10,2)");
        builder.Property(d => d.Subtotal).HasColumnType("decimal(10,2)");

        // Un producto aparece una sola vez por pedido (las cantidades se suman)
        builder.HasIndex(d => new { d.IdPedido, d.IdProducto }).IsUnique();

        builder.HasOne(d => d.Pedido)
               .WithMany(p => p.Detalles)
               .HasForeignKey(d => d.IdPedido)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Producto)
               .WithMany()
               .HasForeignKey(d => d.IdProducto)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
