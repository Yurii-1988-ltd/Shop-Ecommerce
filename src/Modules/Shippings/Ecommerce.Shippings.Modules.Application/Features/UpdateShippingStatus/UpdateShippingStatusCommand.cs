

namespace Ecommerce.Shippings.Modules.Application.Features.UpdateShippingStatus;

public record class UpdateShippingStatusCommand(Guid ShippingId, string Status
    ) : ICommand<Guid>;                 

