

namespace Ecommerce.Shippings.Modules.Application.Features.StartProcessing
{
    public sealed record StartProcessingCommand(
        Guid ShippingId)
        : ICommand<Guid>;

}
