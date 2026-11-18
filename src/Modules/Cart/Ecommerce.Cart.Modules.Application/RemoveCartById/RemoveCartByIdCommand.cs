
namespace Ecommerce.Cart.Modules.Application.RemoveCartById;

public sealed record RemoveCartByIdCommand(Guid CartId) : ICommand;

