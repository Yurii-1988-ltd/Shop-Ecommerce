using Ecommerce.Application.CQRS;
using Ecommerce.Shippings.Modules.Application.Dto;
using Ecommerce.Shippings.Modules.Domain.Entities;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingMethod;

public record GetShippingMethodByIdQuery(Guid Id): IQuery<ShippingMethodResponse>;