using Ecommerce.Application.CQRS;
using Ecommerce.Shippings.Modules.Application.Dto;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingMethods;

public record GetShippingMethodsQuery(): IQuery<IReadOnlyCollection<ShippingMethodResponse>>;