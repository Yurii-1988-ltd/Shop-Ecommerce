using Ecommerce.Shippings.Modules.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Shippings.Modules.Infrastructure.Database.Configurations;

internal sealed class ShippingMethodConfiguration : IEntityTypeConfiguration<ShippingMethod>
{
    public void Configure(EntityTypeBuilder<ShippingMethod> builder)
    {
        builder.ToTable("Shipping_Method");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // Маппинг объекта-значения Money для цены метода доставки
        builder.OwnsOne(x => x.Price, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("Price")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Игнорируем вычисляемое свойство IsFree, так как оно рассчитывается в коде
        builder.Ignore(x => x.IsFree);

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}