

using Ecommerce.Admin.ApiClients.Shippings.Models;

namespace Ecommerce.Admin.ApiClients.Shippings;

public interface IShippingApiClient
{
    Task<PagedResult<ShippingResponse>?> GetAllAsync(
   int page,
   int pageSize,
   string? status = null,
   string? search = null,
   CancellationToken cancellationToken = default);
 Task<ShippingResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<ShipmentTrackingEventResponse>?> GetTrackingEventsAsync(
    Guid shippingId,
    CancellationToken cancellationToken = default);
    Task StartProcessingAsync(Guid id, CancellationToken cancellationToken = default);

    Task MarkAsReadyForShipmentAsync(Guid id, CancellationToken cancellationToken = default);

    Task MarkAsShippedAsync(Guid id, CancellationToken cancellationToken = default);

    Task MarkInTransitAsync(Guid id, CancellationToken cancellationToken = default);

    Task MarkOutForDeliveryAsync(Guid id, CancellationToken cancellationToken = default);

    Task MarkAsDeliveredAsync(Guid id, CancellationToken cancellationToken = default);

    Task MarkAsFailedAsync(Guid id, CancellationToken cancellationToken = default);

    Task MarkAsReturnedAsync(Guid id, CancellationToken cancellationToken = default);

    Task CancelAsync(Guid id, CancellationToken cancellationToken = default);
}

