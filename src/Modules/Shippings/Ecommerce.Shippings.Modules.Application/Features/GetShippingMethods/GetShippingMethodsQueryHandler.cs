using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Application.Dto;

using Ecommerce.Shippings.Modules.Domain.Repositories;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingMethods;

internal sealed class GetShippingMethodsQueryHandler(IShippingMethodRepository repository) : IQueryHandler<GetShippingMethodsQuery,IReadOnlyCollection<ShippingMethodResponse>>
{
    public async Task<Result<IReadOnlyCollection<ShippingMethodResponse>>> Handle(GetShippingMethodsQuery request, CancellationToken cancellationToken)
    {
        var shippingMethod = await repository.GetAllAsync(cancellationToken);
        var response = shippingMethod.Select(x => x.ToResponse())
            .ToList();
        return response;
    }
}