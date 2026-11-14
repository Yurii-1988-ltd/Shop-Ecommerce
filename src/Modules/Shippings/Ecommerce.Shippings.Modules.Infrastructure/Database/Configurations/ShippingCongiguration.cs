using Ecommerce.Domain.ValueObjects;
using Ecommerce.Shippings.Modules.Domain.Entities;
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




        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        builder.OwnsOne(
          x => x.RecipientDetails,
          recipient =>
          {
              recipient.Property(x => x.RecipientName)
                  .HasColumnName("RecipientName")
                  .IsRequired()
                  .HasMaxLength(200);

              recipient.Property(x => x.RecipientPhone)
                  .HasColumnName("RecipientPhone")
                  .IsRequired()
                  .HasMaxLength(30);

              recipient.Property(x => x.Country)
                  .HasColumnName("Country")
                  .IsRequired()
                  .HasMaxLength(100);

              recipient.Property(x => x.City)
                  .HasColumnName("City")
                  .IsRequired()
                  .HasMaxLength(100);

              recipient.Property(x => x.Street)
                  .HasColumnName("Street")
                  .HasMaxLength(200);

              recipient.Property(x => x.Building)
                  .HasColumnName("Building")
                  .HasMaxLength(50);

              recipient.Property(x => x.PostalCode)
                  .HasColumnName("PostalCode")
                  .HasMaxLength(20);

              recipient.Property(x => x.DeliveryPoint)
                  .HasColumnName("DeliveryPoint")
                  .HasMaxLength(300);
          });

        builder.Navigation(x => x.RecipientDetails).IsRequired();


        builder.Property(x => x.TrackingNumber).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => x.ShippingMethodId);
        builder.HasIndex(x => x.TrackingNumber);
    }
}