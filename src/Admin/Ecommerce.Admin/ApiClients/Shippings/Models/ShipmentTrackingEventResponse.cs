namespace Ecommerce.Admin.ApiClients.Shippings.Models;

public sealed record ShipmentTrackingEventResponse(
    Guid Id,
    string Status,
    string Description,
    string? Location,
    DateTime OccurredAtUtc
);
