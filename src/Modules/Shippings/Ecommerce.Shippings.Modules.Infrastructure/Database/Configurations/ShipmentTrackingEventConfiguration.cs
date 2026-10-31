using Ecommerce.Shippings.Modules.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Shippings.Modules.Infrastructure.Database.Configurations;

internal class ShipmentTrackingEventConfiguration : IEntityTypeConfiguration<ShipmentTrackingEvent>
{
    public void Configure(EntityTypeBuilder<ShipmentTrackingEvent> builder)
    {
        builder.ToTable("Shipment_Tracking_Events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShipmentId)
            .IsRequired();

        // Сохраняем энам как строку в БД для читаемости
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Location)
            .HasMaxLength(200);

        builder.Property(x => x.OccurredAt)
            .IsRequired();

        builder.HasIndex(x => x.ShipmentId);
    }
}