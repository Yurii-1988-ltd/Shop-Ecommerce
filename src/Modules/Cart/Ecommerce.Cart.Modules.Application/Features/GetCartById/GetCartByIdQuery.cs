
namespace Ecommerce.Cart.Modules.Application.Features.GetCartById;



public sealed record GetCartByIdQuery(Guid CartId)
    : IQuery<CartResponse>;
