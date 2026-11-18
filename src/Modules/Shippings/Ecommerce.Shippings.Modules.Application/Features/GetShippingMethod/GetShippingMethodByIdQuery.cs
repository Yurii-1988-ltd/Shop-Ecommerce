

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingMethod;

public record GetShippingMethodByIdQuery(Guid Id): IQuery<ShippingMethodResponse>;