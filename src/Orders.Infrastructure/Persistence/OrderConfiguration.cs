using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Orders;

namespace Orders.Infrastructure.Persistence;

// Eşleme Domain'de değil burada (Fluent API). Domain sınıfları EF attribute'u taşımaz.
internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>, IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();
        builder.Property(o => o.Currency).HasMaxLength(3).IsRequired();
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

        // Total hesaplanan değer, DomainEvents ise bellekte bekleyen olaylar: ikisinin de kolonu yok.
        builder.Ignore(o => o.Total);
        builder.Ignore(o => o.DomainEvents);

        // Items public olarak salt okunur; EF _items alanına yazar.
        builder.HasMany(o => o.Items).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();
        builder.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
        builder.Ignore(i => i.Total);

        // Money bir value object: ayrı tablo değil, OrderItems'ın kolonları olarak saklanır.
        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("UnitPrice").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3);
        });
        builder.Navigation(i => i.UnitPrice).IsRequired();
    }
}
