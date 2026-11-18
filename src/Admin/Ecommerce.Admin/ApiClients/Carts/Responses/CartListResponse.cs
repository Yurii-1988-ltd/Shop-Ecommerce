namespace Ecommerce.Admin.ApiClients.Carts.Responses;

public sealed record CartListResponse(
    Guid Id,
    Guid? CustomerId,
    Guid? GuestId,
    int TotalItems,
    decimal TotalAmount,
    string Currency);