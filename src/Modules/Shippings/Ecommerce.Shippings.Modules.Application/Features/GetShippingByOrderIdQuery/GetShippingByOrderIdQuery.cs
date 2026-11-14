namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingByOrderIdQuery;

public sealed record GetShippingByOrderIdQuery(Guid OrderId)
    : IQuery<ShippingResponse>;