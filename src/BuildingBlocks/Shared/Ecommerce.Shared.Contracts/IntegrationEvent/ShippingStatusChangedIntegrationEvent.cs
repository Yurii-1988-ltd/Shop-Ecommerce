public sealed record ShippingStatusChangedIntegrationEvent(
    Guid ShippingId,
    Guid OrderId,
    string Status,
    string? TrackingNumber,
    DateTime UpdatedAtUtc
);