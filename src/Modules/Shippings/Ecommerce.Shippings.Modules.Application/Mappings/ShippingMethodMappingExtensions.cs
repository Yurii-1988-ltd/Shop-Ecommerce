using Ecommerce.Shippings.Modules.Application.Dto;
using Ecommerce.Shippings.Modules.Domain.Entities;

namespace Ecommerce.Shippings.Modules.Application.Mappings;

public static  class ShippingMethodMappingExtensions
{
    public static ShippingMethodResponse ToResponse(
        this ShippingMethod shippingMethod)
    {
        return new ShippingMethodResponse(
            shippingMethod.Id,
            shippingMethod.Name,
            shippingMethod.Code,
            shippingMethod.Description,
            shippingMethod.Price.Amount,
            shippingMethod.Price.Currency,
            shippingMethod.IsFree,
            shippingMethod.IsActive);
    }
}