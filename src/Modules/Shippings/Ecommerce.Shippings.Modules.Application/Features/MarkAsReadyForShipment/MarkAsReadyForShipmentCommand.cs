
namespace Ecommerce.Shippings.Modules.Application.Features.MarkAsReadyForShipment;

public sealed record MarkAsReadyForShipmentCommand(
    Guid ShippingId)
    : ICommand<Guid>;
