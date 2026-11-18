namespace Ecommerce.Shippings.Modules.Application.Dto;

public record ShippingMethodResponse( Guid Id,
    string Name,
    string Code,
    string? Description,
    decimal Price,
    string Currency,
    bool IsFree,
    bool IsActive);