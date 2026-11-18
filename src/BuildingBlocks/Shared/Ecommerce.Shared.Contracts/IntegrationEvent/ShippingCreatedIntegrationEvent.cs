

namespace Ecommerce.Shared.Contracts.IntegrationEvent;

public sealed record ShippingCreatedIntegrationEvent(
    Guid ShippingId,
    Guid OrderId
);
