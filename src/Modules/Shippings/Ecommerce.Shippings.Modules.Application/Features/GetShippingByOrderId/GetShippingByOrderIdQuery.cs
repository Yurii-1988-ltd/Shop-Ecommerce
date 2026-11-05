using Ecommerce.Application.CQRS;
using Ecommerce.Shippings.Modules.Application.Dto;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingByOrderId;

public record GetShippingByOrderIdQuery(Guid OrderId): IQuery<ShippingResponse>;