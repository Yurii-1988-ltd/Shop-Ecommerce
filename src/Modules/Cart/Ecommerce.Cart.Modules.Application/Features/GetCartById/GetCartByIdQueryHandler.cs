

using Ecommerce.Cart.Modules.Application.Features.Cart.Mapping;

namespace Ecommerce.Cart.Modules.Application.Features.GetCartById;

internal sealed class GetCartByIdQueryHandler(
    ICartRepository cartRepository)
    : IQueryHandler<GetCartByIdQuery, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(
        GetCartByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
            return CartErrors.NotFound(request.CartId);

        var cart = await cartRepository.GetByIdAsync(
            request.CartId,
            cancellationToken);

        if (cart is null)
            return CartErrors.NotFound(request.CartId);

        return Result.Success(cart.ToResponse());
    }
}