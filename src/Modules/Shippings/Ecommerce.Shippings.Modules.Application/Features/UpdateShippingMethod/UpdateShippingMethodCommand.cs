using Ecommerce.Application.CQRS;

namespace Ecommerce.Shippings.Modules.Application.Features.UpdateShippingMethod;

public record UpdateShippingMethodCommand(Guid Id, string Name,
    string Code,
    string Description,
    decimal Price,
    string Currency): ICommand<Guid>;