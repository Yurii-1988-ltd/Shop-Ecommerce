

using Ecommerce.Application.CQRS;
using Ecommerce.Shippings.Modules.Application.Dto;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippings;

public sealed record GetShippingsQuery() : IQuery<IReadOnlyList<ShippingResponse>>;

