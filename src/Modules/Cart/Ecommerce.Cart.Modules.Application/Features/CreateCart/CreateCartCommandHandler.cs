
namespace Ecommerce.Cart.Modules.Application.Features.CreateCart;

internal sealed class CreateCartCommandHandler(
    ICartRepository cartRepository)
    : ICommandHandler<CreateCartCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateCartCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CustomerId.HasValue && request.GuestId.HasValue)
            return CartErrors.OwnerConflict;

        if (!request.CustomerId.HasValue && !request.GuestId.HasValue)
            return CartErrors.OwnerRequired;

        if (request.CustomerId is Guid customerId)
        {
            var existingCart = await cartRepository.GetByCustomerIdAsync(
                customerId,
                cancellationToken);

            if (existingCart is not null)
                return CartErrors.AlreadyExists(customerId);

            var cartResult = Domain.Entities.Cart.CreateForCustomer(customerId);

            if (cartResult.IsFailure)
                return cartResult.Error;

            await cartRepository.InsertAsync(
                cartResult.Value,
                cancellationToken);

            return cartResult.Value.Id;
        }

        var guestId = request.GuestId!.Value;

        var existingGuestCart = await cartRepository.GetByGuestIdAsync(
            guestId,
            cancellationToken);

        if (existingGuestCart is not null)
            return existingGuestCart.Id;

        var guestCartResult = Domain.Entities.Cart.CreateForGuest(guestId);

        if (guestCartResult.IsFailure)
            return guestCartResult.Error;

        await cartRepository.InsertAsync(
            guestCartResult.Value,
            cancellationToken);

        return guestCartResult.Value.Id;
    }
}
