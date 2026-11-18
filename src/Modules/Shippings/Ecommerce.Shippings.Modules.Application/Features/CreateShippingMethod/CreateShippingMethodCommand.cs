using Ecommerce.Application.CQRS;

namespace Ecommerce.Shippings.Modules.Application.Features.CreateShippingMethod;

public record CreateShippingMethodCommand(string Name,
    string Code,
    string? Description,
    decimal Price,
    string Currency): ICommand<Guid>;