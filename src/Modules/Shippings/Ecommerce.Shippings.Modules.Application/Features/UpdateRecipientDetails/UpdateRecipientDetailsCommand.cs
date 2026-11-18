namespace Ecommerce.Shippings.Modules.Application.Features.UpdateRecipientDetails;

public sealed record UpdateRecipientDetailsCommand(
    Guid ShippingId,
    string RecipientName,
    string RecipientPhone,
    string Country,
    string City,
    string? Street,
    string? Building,
    string? PostalCode,
    string? DeliveryPoint) : ICommand<Guid>;