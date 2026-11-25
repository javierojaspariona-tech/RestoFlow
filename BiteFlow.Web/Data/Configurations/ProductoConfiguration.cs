using BiteFlow.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteFlow.Web.Data.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Producto");
        builder.HasKey(p => p.IdProducto);
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Descripcion).HasMaxLength(300);
        builder.Property(p => p.Precio).HasColumnType("decimal(10,2)");

        // No se repite el nombre de un producto dentro de la misma categoría
        builder.HasIndex(p => new { p.IdCategoria, p.Nombre }).IsUnique();

        builder.HasOne(p => p.Categoria)
               .WithMany(c => c.Productos)
               .HasForeignKey(p => p.IdCategoria)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
