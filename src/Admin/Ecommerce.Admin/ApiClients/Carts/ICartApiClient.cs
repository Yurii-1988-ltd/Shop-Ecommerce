
using Ecommerce.Admin.ApiClients.Carts.Contracts;
using Ecommerce.Admin.ApiClients.Carts.Responses;
using Ecommerce.Application.Pagination;

namespace Ecommerce.Admin.ApiClients.Carts;

public interface ICartApiClient
{
    // Get all carts
    Task<PagedResult<CartListResponse>?> GetAllAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    // Get cart by Cart.Id
    Task<CartResponse?> GetByIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default);

    // Get cart by CustomerId
    Task<CartResponse?> GetAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    // Create cart
    Task<Guid> CreateAsync(
        CreateCartRequest request,
        CancellationToken cancellationToken = default);

    // Add item
    Task AddItemAsync(
        Guid customerId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default);

    // Change item quantity
    Task ChangeQuantityAsync(
        Guid customerId,
        Guid productId,
        ChangeCartItemQuantityRequest request,
        CancellationToken cancellationToken = default);

    // Remove item
    Task RemoveItemAsync(
        Guid customerId,
        Guid productId,
        CancellationToken cancellationToken = default);

    // Clear cart
    Task ClearAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    // Remove cart by CustomerId
    Task RemoveFromCartAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    // Remove cart by Cart.Id
    Task RemoveByIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default);

    // Apply coupon
    Task ApplyCouponAsync(
        Guid customerId,
        ApplyCouponRequest request,
        CancellationToken cancellationToken = default);

    // Remove coupon
    Task RemoveCouponAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
   
}
