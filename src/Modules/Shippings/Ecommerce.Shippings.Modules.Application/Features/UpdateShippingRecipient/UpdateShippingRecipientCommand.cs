

namespace Ecommerce.Shippings.Modules.Application.Features.UpdateShippingRecipient;

public sealed record UpdateShippingRecipientCommand(
    Guid ShippingId,
    string RecipientName,
    string RecipientPhone,
    string Country,
    string City,
    string? Street,
    string? Building,
    string? PostalCode,
    string? DeliveryPoint
) : ICommand<Guid>;