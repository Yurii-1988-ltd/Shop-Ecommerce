using Ecommerce.Cart.Modules.Application.RemoveCartById;
using Ecommerce.Cart.Modules.Domain.Errors;
using Ecommerce.Cart.Modules.Domain.Repositories;

namespace Ecommerce.Cart.Modules.Application.Features.RemoveCartById;

internal sealed class RemoveCartByIdCommandHandler(
    ICartRepository cartRepository)
    : ICommandHandler<RemoveCartByIdCommand>
{
    public async Task<Result> Handle(
        RemoveCartByIdCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
        {
            return CartErrors.NotFound(request.CartId);
        }

        var cart = await cartRepository.GetByIdAsync(
            request.CartId,
            cancellationToken);

        if (cart is null)
        {
            return CartErrors.NotFound(request.CartId);
        }

        await cartRepository.DeleteAsync(
            cart.Id,
            cancellationToken);

        return Result.Success();
    }
}