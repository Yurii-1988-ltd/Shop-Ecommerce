

namespace Ecommerce.Shippings.Modules.Application.Features.MarkAsFailed;

public sealed record MarkAsFailedCommand(
    Guid ShippingId) : ICommand<Guid>;

