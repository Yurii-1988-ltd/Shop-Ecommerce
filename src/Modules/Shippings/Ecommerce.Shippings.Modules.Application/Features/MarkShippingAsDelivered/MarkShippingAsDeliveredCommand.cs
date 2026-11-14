

namespace Ecommerce.Shippings.Modules.Application.Features.MarkShippingAsDelivered;

public sealed record MarkShippingAsDeliveredCommand(
    Guid ShippingId) : ICommand<Guid>;

