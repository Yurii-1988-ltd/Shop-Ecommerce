

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingById;

public sealed record GetShippingByIdQuery(Guid Id)
    : IQuery<ShippingResponse>;