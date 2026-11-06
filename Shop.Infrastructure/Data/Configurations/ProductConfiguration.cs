using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");


        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(1000);

        builder.Property(p => p.Stock).IsRequired();
        builder.OwnsOne(
            p => p.Price,
            money =>
            {
                money.Property(mp => mp.Amount)
                    .HasColumnName("PriceAmount")
                    .HasPrecision(18, 2);

                money.Property(mp => mp.Currency)
                    .HasConversion<string>()
                    .HasColumnName("PriceCurrency")
                    .HasMaxLength(3);
            });
        builder.Property(p => p.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
