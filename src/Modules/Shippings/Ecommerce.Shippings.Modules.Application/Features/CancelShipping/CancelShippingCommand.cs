

using Ecommerce.Application.CQRS;

namespace Ecommerce.Shippings.Modules.Application.Features.CancelShipping;

public sealed record CancelShippingCommand(Guid ShippingId) : ICommand<Guid>;

