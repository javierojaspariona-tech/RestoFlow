using BiteFlow.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteFlow.Web.Data.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("Pago");
        builder.HasKey(p => p.IdPago);
        builder.Property(p => p.Monto).HasColumnType("decimal(10,2)");

        builder.Property(p => p.MetodoPago)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(p => p.Estado)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(p => p.NumeroComprobante)
               .IsRequired()
               .HasMaxLength(20)
               .IsUnicode(false);
        builder.HasIndex(p => p.NumeroComprobante).IsUnique();

        // Un pedido = un solo pago (relación 1 a 0..1 con índice único en IdPedido)
        builder.HasOne(p => p.Pedido)
               .WithOne(ped => ped.Pago)
               .HasForeignKey<Pago>(p => p.IdPedido)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(p => p.IdPedido).IsUnique();

        builder.HasOne(p => p.Cajero)
               .WithMany()
               .HasForeignKey(p => p.IdUsuarioCajero)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
