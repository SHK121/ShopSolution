using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.Status).IsRequired();

        builder.Property(x => x.OrderCurrency)
            .HasConversion<string>()
            .HasMaxLength(3);

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasMany<OrderItem>(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.ToTable("Orders");
            
    }
}
