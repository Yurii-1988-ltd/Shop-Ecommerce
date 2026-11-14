using Ecommerce.Shippings.Modules.Application.Features.GetShippingMethod;

internal sealed class GetShippingMethodByIdQueryHandler(
    IShippingMethodRepository repository)
    : IQueryHandler<GetShippingMethodByIdQuery, ShippingMethodResponse>
{
    public async Task<Result<ShippingMethodResponse>> Handle(
        GetShippingMethodByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        var shippingMethod = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (shippingMethod is null)
            return ShippingErrors.ShippingMethodNotFound;

        return shippingMethod.ToResponse();
    }
}