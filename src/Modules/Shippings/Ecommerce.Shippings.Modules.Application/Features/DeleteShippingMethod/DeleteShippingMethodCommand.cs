using Ecommerce.Application.CQRS;

namespace Ecommerce.Shippings.Modules.Application.Features.DeleteShippingMethod;

public sealed record DeleteShippingMethodCommand(Guid Id): ICommand<Guid>;