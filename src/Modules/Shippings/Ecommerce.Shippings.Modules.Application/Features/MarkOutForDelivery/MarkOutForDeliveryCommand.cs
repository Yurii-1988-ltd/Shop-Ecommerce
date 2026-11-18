
namespace Ecommerce.Shippings.Modules.Application.Features.MarkOutForDelivery
{
    public sealed record MarkOutForDeliveryCommand(
    Guid ShippingId)
    : ICommand<Guid>;

}
