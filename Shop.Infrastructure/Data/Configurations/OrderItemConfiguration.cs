using Shop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shop.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId).IsRequired();

        builder.Property(x => x.ProductId).IsRequired();

        builder.Property(x => x.Quantity).IsRequired();

        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);

        builder.ToTable("OrderItems");
       // builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.OwnsOne(
            o => o.UnitPrice,
            up =>
            {
                up.Property(x => x.Amount)
                    .HasColumnName("UnitPriceAmount")
                    .HasPrecision(18, 2);
                up.Property(x => x.Currency)
                    .HasConversion<string>()
                    .HasColumnName("UnitPriceCurrency")
                    .HasMaxLength(3);
            });
    }
}
