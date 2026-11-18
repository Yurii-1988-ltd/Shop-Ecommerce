namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingByOrderIdQuery;

internal sealed class GetShippingByOrderIdQueryHandler(
    IShippingRepository repository)
    : IQueryHandler<GetShippingByOrderIdQuery, ShippingResponse>
{
    public async Task<Result<ShippingResponse>> Handle(
        GetShippingByOrderIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.OrderId == Guid.Empty)
            return ShippingErrors.OrderIdIsRequired;

        var shipping = await repository.GetByOrderIdAsync(
            request.OrderId,
            cancellationToken);

        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.OrderId);

        return shipping.ToResponse();
    }
}