using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Entities;

namespace Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {

        // PROPERTIES

        builder.ToTable("Pedidos", table =>
            {
                table.HasCheckConstraint(
                    "CK_ItemPedido_ClienteNome_MaxLength",
                    "length(\"ClienteNome\") <= 200"
                );

                table.HasCheckConstraint(
                    "CK_ItemPedido_StatusMaxLength",
                    "length(\"Status\") <= 20"
                );
            }
        );

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.ClienteNome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.DataCriacao)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        // NAVIGATION
        builder.Metadata
            .FindNavigation(nameof(Pedido.Itens))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(ip => ip.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}