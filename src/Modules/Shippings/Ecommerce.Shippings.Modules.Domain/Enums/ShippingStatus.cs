

namespace Ecommerce.Shippings.Modules.Domain.Enums;

public enum ShippingStatus
{
    Pending = 1,
    Processing = 2,
    ReadyForShipment = 3,
    Shipped = 4,
    InTransit = 5,
    OutForDelivery = 6,
    Delivered = 7,
    Failed = 8,
    Returned = 9,
    Cancelled = 10
}
