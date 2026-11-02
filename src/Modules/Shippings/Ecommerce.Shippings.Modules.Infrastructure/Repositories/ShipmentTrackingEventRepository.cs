using Ecommerce.Shipings.Modules.Domain.Entities;
using Ecommerce.Shipings.Modules.Domain.Enums;


internal sealed class ShipmentTrackingEventRepository(ShippingContext context) : IShipmentTrackingEventRepository
{
    
    public async Task<List<ShipmentTrackingEvent>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await context.shipmentTrackingEvents
            .Where(e => e.ShipmentId == shipmentId)
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShipmentTrackingEvent?> GetLatestByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await context.shipmentTrackingEvents
            .Where(e => e.ShipmentId == shipmentId)
            .OrderByDescending(e => e.OccurredAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid shipmentId, ShippingStatus status, DateTime occurredAt, CancellationToken cancellationToken = default)
    {
        return await context.shipmentTrackingEvents
            .AnyAsync(e => e.ShipmentId == shipmentId && e.Status == status && e.OccurredAt == occurredAt, cancellationToken);
    }

    public async Task AddAsync(ShipmentTrackingEvent trackingEvent, CancellationToken cancellationToken = default)
    {
        await context.shipmentTrackingEvents.AddAsync(trackingEvent, cancellationToken);
    }
}