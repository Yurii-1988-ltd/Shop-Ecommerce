using Ecommerce.Shippings.Modules.Domain.Entities;

namespace Ecommerce.Shippings.Modules.Application.Dto.Extensions;

public static class ShippingMappingExtensions
{
    public static ShippingResponse ToResponse(this Shipping shipping)
    {
        var recipient = shipping.RecipientDetails;

        return new ShippingResponse(
            shipping.Id,
            shipping.OrderId,
            shipping.ShippingMethodId,
            shipping.ShippingMethodName,
            shipping.ShippingCost.Amount,
            shipping.ShippingCost.Currency,
            shipping.Status.ToString(),
            recipient.RecipientName,
            recipient.RecipientPhone,
            recipient.Country,
            recipient.City,
            recipient.Street,
            recipient.Building,
            recipient.PostalCode,
            recipient.DeliveryPoint,
            shipping.TrackingNumber,
            shipping.Notes,
            shipping.CreatedAtUtc,
            shipping.ShippedAtUtc,
            shipping.DeliveredAtUtc,
            shipping.UpdatedAtUtc);
    }
}