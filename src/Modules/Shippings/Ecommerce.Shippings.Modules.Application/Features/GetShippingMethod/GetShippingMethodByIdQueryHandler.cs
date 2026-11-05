using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Application.Dto;
using Ecommerce.Shippings.Modules.Domain.Repositories;
using Ecommerce.Shippings.Modules.Application.Mappings;
using Ecommerce.Shippings.Modules.Domain.Errors;

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

       

        return shippingMethod.ToResponse();
    }
}