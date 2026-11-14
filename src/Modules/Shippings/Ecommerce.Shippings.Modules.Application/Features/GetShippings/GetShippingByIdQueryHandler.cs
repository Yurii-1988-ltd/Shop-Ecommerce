using Ecommerce.Shippings.Modules.Application.Features.GetShippingById;

public sealed class GetShippingByIdQueryHandler(
    IShippingRepository shippingRepository)
    : IQueryHandler<GetShippingByIdQuery, ShippingResponse>
{
    public async Task<Result<ShippingResponse>> Handle(
        GetShippingByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            return Result.Failure<ShippingResponse>(
                ShippingErrors.GuidEmpty);

        var shipping = await shippingRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (shipping is null)
            return Result.Failure<ShippingResponse>(
                ShippingErrors.ShippingNotFound(request.Id));

        return Result.Success(shipping.ToResponse());
    }
}