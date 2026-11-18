using Ecommerce.Application.CQRS;

namespace Ecommerce.Shippings.Modules.Application.Features.DeactivateShippingMethod;

public record DeactivateShippingMethodCommand(Guid Id): ICommand<Guid>;