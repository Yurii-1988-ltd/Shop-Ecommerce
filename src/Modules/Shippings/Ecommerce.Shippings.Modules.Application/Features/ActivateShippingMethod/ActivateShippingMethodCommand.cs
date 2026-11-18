using Ecommerce.Application.CQRS;

namespace Ecommerce.Shippings.Modules.Application.Features.ActivateShippingMethod;

public sealed record ActivateShippingMethodCommand(Guid Id): ICommand<Guid>;
