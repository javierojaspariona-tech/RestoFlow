using BiteFlow.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteFlow.Web.Data.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedido");
        builder.HasKey(p => p.IdPedido);

        builder.Property(p => p.Estado)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(p => p.Total).HasColumnType("decimal(10,2)");
        builder.Property(p => p.RowVersion).IsRowVersion();

        builder.HasOne(p => p.Cliente)
               .WithMany(c => c.Pedidos)
               .HasForeignKey(p => p.IdCliente)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.Estado, p.FechaPedido });
    }
}
