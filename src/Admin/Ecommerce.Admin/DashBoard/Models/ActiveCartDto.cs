namespace Ecommerce.Admin.DashBoard.Models;

public sealed record ActiveCartDto(
    Guid Id,
    string CustomerName,
    int TotalQuantity,
    decimal TotalAmount,
    string Currency,
    DateTime UpdatedAtUtc);
