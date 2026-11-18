namespace Ecommerce.Admin.Components.Orders.Models;

public sealed record RecentOrderDto(
    Guid Id,
    string OrderNumber,
    DateTime CreatedAtUtc,
    string Status,
    decimal TotalAmount,
    string Currency);