using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Application.Dto;
using Ecommerce.Shippings.Modules.Domain.Repositories;
using Ecommerce.Shipings.Modules.Domain.Errors;
using Ecommerce.Shippings.Modules.Application.Mappings;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingMethod;

internal sealed class GetShippingMethodByIdQueryHandler(
    IShippingMethodRepository repository)
    : IQueryHandler<GetShippingMethodByIdQuery, ShippingMethodResponse>
{
    public async Task<Result<ShippingMethodResponse>> Handle(
        GetShippingMethodByIdQuery request,
        CancellationToken cancellationToken)
    {
        var shippingMethod = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (shippingMethod is null)
            return ShippingErrors.ShippingMethodNotFound;

        var response = shippingMethod.ToResponse();

        return response;
    }
}