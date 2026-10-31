using Ecommerce.Domain.ValueObjects;
using Ecommerce.Shippings.Modules.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Shippings.Modules.Infrastructure.Database.Configurations;

internal sealed class ShippingConfiguration : IEntityTypeConfiguration<Shipping>
{
    public void Configure(EntityTypeBuilder<Shipping> builder)
    {
        builder.ToTable("Shippings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.HasIndex(x => x.OrderId)
            .IsUnique();

        builder.Property(x => x.ShippingMethodId)
            .IsRequired();

        builder.Property(x => x.ShippingMethodName)
            .IsRequired()
            .HasMaxLength(150);

        builder.OwnsOne<Money>(x => x.ShippingCost, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("ShippingCost")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });


        builder.Property(x => x.Currency)
            .HasColumnName("Currency")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.RecipientName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.RecipientPhone).IsRequired().HasMaxLength(30);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(100);
        builder.Property(x => x.City).IsRequired().HasMaxLength(100);

        builder.Property(x => x.Street).HasMaxLength(200);
        builder.Property(x => x.Building).HasMaxLength(50);
        builder.Property(x => x.PostalCode).HasMaxLength(20);
        builder.Property(x => x.DeliveryPoint).HasMaxLength(300);
        builder.Property(x => x.TrackingNumber).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => x.ShippingMethodId);
        builder.HasIndex(x => x.TrackingNumber);
    }
}