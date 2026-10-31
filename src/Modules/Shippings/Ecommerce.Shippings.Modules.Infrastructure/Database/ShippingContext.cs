

using Ecommerce.Shippings.Modules.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Shippings.Modules.Infrastructure.Database;

public sealed class ShippingContext : DbContext
{
    public ShippingContext(DbContextOptions<ShippingContext>options):base(options)
    {
        
    }
    public DbSet<Shipping> Shippings => Set<Shipping>();
    public DbSet<ShippingMethod> ShippingMethod => Set<ShippingMethod>();
    public DbSet<ShipmentTrackingEvent> shipmentTrackingEvents => Set<ShipmentTrackingEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShippingContext).Assembly);
    }
}
