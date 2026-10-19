using Ecommerce.Order.Modules.Application.Features.Responses;
using Ecommerce.Order.Modules.Domain.Errors;
using MediatR;

namespace Ecommerce.Order.Modules.Application.Features.GetBySlug;

internal sealed class GetBySlugQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetBySlugQuery, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(
        GetBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetBySlugAsync(request.slug, cancellationToken);

        if (order is null)
        {
            return OrderErrors.NotFound(request.slug);
        }
      
        var response = order.ToResponse(user: null);

  
        return Result.Success(response);
    }
}