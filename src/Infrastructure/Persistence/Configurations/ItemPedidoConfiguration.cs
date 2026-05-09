using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Entities;

namespace Infrastructure.Persistence.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        // PROPERTIES
        builder.ToTable("ItensPedido", table =>
        {
            table.HasCheckConstraint(
                "CK_ItemPedido_Quantidade",
                "\"Quantidade\" > 0"
            );

            table.HasCheckConstraint(
                "CK_ItemPedido_PrecoUnitario",
                "\"PrecoUnitario\" > 0"
            );
        });

        builder.HasKey(ip => ip.Id);

        builder.Property(ip => ip.Id)
            .ValueGeneratedNever();

        builder.Property(ip => ip.ProdutoNome)
             .IsRequired()
             .HasMaxLength(200);

        builder.Property(ip => ip.Quantidade)
            .IsRequired();

        builder.Property(ip => ip.PrecoUnitario)
            .IsRequired()
            .HasPrecision(18,2);

        builder.Ignore( ip => ip.Subtotal);
    }
}