
namespace Ecommerce.Shippings.Modules.Application.Dto;



public sealed record ChangeShippingStatusDto(
    Guid ShippingId,
    string NewStatus,          // Например: "Processing", "Shipped", "Delivered", "Cancelled"
    string? TrackingNumber     // Трекинг-номер (обязателен, когда посылка отправляется)
);