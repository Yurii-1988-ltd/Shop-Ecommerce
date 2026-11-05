using Ecommerce.Shippings.Modules.Domain.Entities;

namespace Ecommerce.Shippings.Modules.Application.Dto.Extensions;

public static class ShippingMappingExtensions
{
    public static ShippingResponse ToResponse(this Shipping shipping)
    {
        return new ShippingResponse(
            shipping.Id,
            shipping.OrderId,
            shipping.ShippingMethodId,
            shipping.ShippingMethodName,
            shipping.ShippingCost.Amount,
            shipping.ShippingCost.Currency,
            shipping.Status.ToString(),
            shipping.RecipientName,
            shipping.RecipientPhone,
            shipping.Country,
            shipping.City,
            shipping.Street,
            shipping.Building,
            shipping.PostalCode,
            shipping.DeliveryPoint,
            shipping.TrackingNumber,
            shipping.Notes,
            shipping.CreatedAtUtc,
            shipping.ShippedAtUtc,
            shipping.DeliveredAtUtc,
            shipping.UpdatedAtUtc);
    }
}