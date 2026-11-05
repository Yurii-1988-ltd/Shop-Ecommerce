
using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Application.Dto;
using Ecommerce.Shippings.Modules.Application.Dto.Extensions;
using Ecommerce.Shippings.Modules.Domain.Repositories;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippings;

internal sealed class GetShippingsQueryHandler(IShippingRepository repository) : IQueryHandler<GetShippingsQuery, IReadOnlyList<ShippingResponse>>
{
    public async Task<Result<IReadOnlyList<ShippingResponse>>> Handle(GetShippingsQuery request, CancellationToken cancellationToken)
    {
        var shippings =await repository.GetAllAsync(cancellationToken);
        return shippings.Select(x=>x.ToResponse())
            .ToList();
    }
}
