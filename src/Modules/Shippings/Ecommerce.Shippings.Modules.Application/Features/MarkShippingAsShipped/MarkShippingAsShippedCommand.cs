

namespace Ecommerce.Shippings.Modules.Application.Features.MarkShippingAsShipped;

public sealed record MarkShippingAsShippedCommand(
    Guid ShippingId)
    : ICommand<Guid>;
