using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Application.Dto;
using Ecommerce.Shippings.Modules.Application.Dto.Extensions;
using Ecommerce.Shippings.Modules.Domain.Errors;
using Ecommerce.Shippings.Modules.Domain.Repositories;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippingByOrderId;

internal sealed class GetShippingByOrderIdQueryHandler(IShippingRepository repository) : IQueryHandler<GetShippingByOrderIdQuery,ShippingResponse>
{
    public async Task<Result<ShippingResponse>> Handle(GetShippingByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var shipping = await repository.GetByOrderIdAsync(request.OrderId, cancellationToken);
       
        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.OrderId);

        return shipping.ToResponse();
    }
    
}