

namespace Ecommerce.Shippings.Modules.Application.Features.MarkAsReturned
{
    public sealed record MarkAsReturnedCommand(
        Guid ShippingId) : ICommand<Guid>;

}
