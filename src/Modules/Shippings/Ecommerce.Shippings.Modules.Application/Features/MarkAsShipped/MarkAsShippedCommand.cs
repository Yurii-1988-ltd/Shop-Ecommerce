

namespace Ecommerce.Shippings.Modules.Application.Features.MarkAsShipped;

public sealed record MarkAsShippedCommand(Guid ShippingId) : ICommand<Guid>;
