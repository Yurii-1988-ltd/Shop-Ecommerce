
using Ecommerce.Modules.Users.Contracts.Dto;
using Ecommerce.Order.Modules.Application.Features.Responses;
using Ecommerce.Shared.Contracts.Orders;

internal static class OrderMappings
{
    public static OrderResponse ToResponse(
        this Order order,
        UserDto? user)
    {
        var (totalAmount, currency) = GetTotal(order);

        var customerName = GetCustomerName(order, user);

        return new OrderResponse(
            order.Id,
            order.OrderNumber,
            order.Slug,
            order.CustomerId,
            customerName,
            order.CustomerEmail,
            order.Status,
            new AddressResponse(
                order.ShippingAddress.FirstName,
                order.ShippingAddress.LastName,
                order.ShippingAddress.Country,
                order.ShippingAddress.City,
                order.ShippingAddress.Street,
                order.ShippingAddress.ZipCode),
            order.Items
                .Select(item => new OrderItemResponse(
                    item.Id,
                    item.ProductId,
                    item.ProductName,
                    item.SKU,
                    item.UnitPrice.Amount,
                    item.UnitPrice.Currency,
                    item.Quantity,
                    item.TotalPrice.Amount))
                .ToList(),
            order.TotalQuantity,
            totalAmount,
            currency,
            order.CreatedAtUtc,
            order.PaidAtUtc,
            order.ShippedAtUtc,
            order.CancelledAtUtc,
            order.CancellationReason);
    }

    internal static OrderListResponse ToListResponse(
        this Order order,
        UserDto? user)
    {
        var (totalAmount, currency) = GetTotal(order);

        var customerName = GetCustomerName(order, user);

        return new OrderListResponse(
            order.Id,
            order.OrderNumber,
            order.Slug,
            order.CustomerId,
            customerName,
            order.Status.ToString(),
            order.TotalQuantity,
            totalAmount,
            currency,
            order.CreatedAtUtc);
    }

    internal static OrderListResponse ToMyOrderListResponse(
        this Order order)
    {
        var (totalAmount, currency) = GetTotal(order);

        return new OrderListResponse(
            order.Id,
            order.OrderNumber,
            order.Slug,
            order.CustomerId,
            CustomerName: null,
            Status: order.Status.ToString(),
            TotalQuantity: order.TotalQuantity,
            TotalAmount: totalAmount,
            Currency: currency,
            CreatedAtUtc: order.CreatedAtUtc);
    }

    private static (decimal Amount, string Currency) GetTotal(
        Order order)
    {
        var result = order.GetTotalAmount();

        return result.IsSuccess
            ? (result.Value.Amount, result.Value.Currency)
            : (0m, order.Currency);
    }

    private static string GetCustomerName(
        Order order,
        UserDto? user)
    {
        if (user is not null)
        {
            return $"{user.FirstName} {user.LastName}".Trim();
        }

        if (!string.IsNullOrWhiteSpace(
                order.ShippingAddress?.FirstName))
        {
            return $"{order.ShippingAddress.FirstName} " +
                   $"{order.ShippingAddress.LastName}".Trim();
        }

        return "Guest";
    }
}
