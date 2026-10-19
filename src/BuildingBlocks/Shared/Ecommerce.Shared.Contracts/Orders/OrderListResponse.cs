namespace Ecommerce.Shared.Contracts.Orders;
public sealed record OrderListResponse(
    Guid Id,
    string OrderNumber,
    string Slug,
    Guid? CustomerId,
    string? CustomerName,
    string Status,
    int TotalQuantity,
    decimal TotalAmount,
    string Currency,
    DateTime CreatedAtUtc);