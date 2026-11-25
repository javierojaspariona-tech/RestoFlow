using BiteFlow.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiteFlow.Web.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Cliente");
        builder.HasKey(c => c.IdCliente);
        builder.Property(c => c.Nombres).IsRequired().HasMaxLength(120);
        builder.Property(c => c.Apellidos).IsRequired().HasMaxLength(120);
        builder.Property(c => c.Dni).IsRequired().HasMaxLength(8).IsUnicode(false);
        builder.Property(c => c.Telefono).HasMaxLength(20);
        builder.Property(c => c.Correo).HasMaxLength(120);

        // Un DNI = un cliente
        builder.HasIndex(c => c.Dni).IsUnique();

        // Relación 1 a 0..1 con Usuario. El índice único filtrado evita que dos clientes
        // compartan el mismo usuario, pero permite varios clientes sin usuario (IdUsuario NULL).
        builder.HasOne(c => c.Usuario)
               .WithOne(u => u.Cliente)
               .HasForeignKey<Cliente>(c => c.IdUsuario)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.IdUsuario)
               .IsUnique()
               .HasDatabaseName("UX_Cliente_IdUsuario")
               .HasFilter("[IdUsuario] IS NOT NULL");
    }
}
