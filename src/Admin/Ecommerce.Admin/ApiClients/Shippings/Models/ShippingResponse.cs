namespace Ecommerce.Admin.ApiClients.Shippings.Models;

public sealed record ShippingResponse(
    Guid Id,
    Guid OrderId,
    Guid ShippingMethodId,
    string ShippingMethodName,
    decimal ShippingCost,
    string Currency,
    string Status,
    string RecipientName,
    string RecipientPhone,
    string Country,
    string City,
    string? Street,
    string? Building,
    string? PostalCode,
    string? DeliveryPoint,
    string? TrackingNumber,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime? ShippedAtUtc,
    DateTime? DeliveredAtUtc,
    DateTime? UpdatedAtUtc);
