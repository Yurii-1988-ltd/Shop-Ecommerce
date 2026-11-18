using Ecommerce.Shipings.Modules.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Shipings.Modules.Infrastructure.Database.Configurations;

internal sealed class ShipmentTrackingEventConfiguration
    : IEntityTypeConfiguration<ShipmentTrackingEvent>
{
    public void Configure(EntityTypeBuilder<ShipmentTrackingEvent> builder)
    {
        builder.ToTable("Shipment_Tracking_Events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShipmentId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Location)
            .HasMaxLength(200);

        builder.Property(x => x.OccurredAtUtc)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.ShipmentId,
            x.OccurredAtUtc
        });
    }
}