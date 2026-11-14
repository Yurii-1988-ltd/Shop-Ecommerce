namespace Ecommerce.Shippings.Modules.Application.Dto;

public sealed record ShipmentTrackingEventResponse(
    Guid Id,
    Guid ShipmentId,
    string Status,
    string Description,
    DateTime OccurredAtUtc);