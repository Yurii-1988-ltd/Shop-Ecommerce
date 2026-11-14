

namespace Ecommerce.Shippings.Modules.Application.Features.MarkInTransit;

public sealed record MarkInTransitCommand(
    Guid ShippingId)
    : ICommand<Guid>;
