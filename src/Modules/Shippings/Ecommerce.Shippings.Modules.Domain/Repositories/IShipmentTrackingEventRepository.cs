using Ecommerce.Shipings.Modules.Domain.Entities;
using Ecommerce.Shipings.Modules.Domain.Enums;

namespace Ecommerce.Shippings.Modules.Domain.Repositories;

public interface IShipmentTrackingEventRepository
{

    Task<List<ShipmentTrackingEvent>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);


    Task<ShipmentTrackingEvent?> GetLatestByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);


    Task<bool> ExistsAsync(Guid shipmentId, ShippingStatus status, DateTime occurredAt, CancellationToken cancellationToken = default);


    Task AddAsync(ShipmentTrackingEvent trackingEvent, CancellationToken cancellationToken = default);
}