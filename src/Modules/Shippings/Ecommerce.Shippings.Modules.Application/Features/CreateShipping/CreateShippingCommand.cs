using Ecommerce.Application.CQRS;

namespace Ecommerce.Shippings.Modules.Application.Features.CreateShipping;

public sealed record CreateShippingCommand(
    Guid OrderId,
    Guid ShippingMethodId,
    string RecipientName,
    string RecipientPhone,
    string Country,
    string City,
    string? Street,
    string? Building,
    string? PostalCode,
    string? DeliveryPoint,
    string? Notes
) : ICommand<Guid>;