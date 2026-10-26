using Ecommerce.Domain.Domain;
using Ecommerce.Order.Modules.Domain.Enums;
using Ecommerce.Order.Modules.Domain.Errors;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Order.Modules.Domain.Entities;

public sealed class Order : Entity
{
    #region fields and constructors
    [BsonElement("Items")]
    private List<OrderItem> _items = new();

    public Guid? CustomerId { get; private set; }
    public string CustomerEmail { get; private set; }
    public string OrderNumber { get; private set; } = default!;
    public string Slug { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public OrderAddress ShippingAddress { get; private set; }
    public string Currency { get; private set; }

    /// <summary>
    /// Exchange rate expressed as UAH per one unit of the order currency
    /// </summary>
    public decimal? ExchangeRate { get; private set; }

    public DateOnly? ExchangeRateDate { get; private set; }

    public DateTime? ExchangeRateCapturedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime? ShippedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public int TotalQuantity => _items.Sum(item => item.Quantity);
    #endregion

    #region static factory methods
    private Order()
    {
    }

    private Order(
        Guid id,
        Guid? customerId,
        string customerEmail,
        string orderNumber,
        string slug,
        OrderAddress shippingAddress,
        string currency = CurrencyConstant.UAH)
    {
        Id = id;
        CustomerId = customerId;
        CustomerEmail = customerEmail;
        OrderNumber = orderNumber;
        Slug = slug;
        ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));

        Currency = string.IsNullOrWhiteSpace(currency)
            ? CurrencyConstant.USD
            : currency.ToUpperInvariant();

        Status = OrderStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Result<Order> Create(
        Guid? customerId,
        string customerEmail,
        string orderNumber,
        OrderAddress shippingAddress,
        string currency = CurrencyConstant.UAH)
    {
        if (customerId.HasValue && customerId.Value == Guid.Empty)
            return OrderErrors.GuidEmpty;

        if (string.IsNullOrWhiteSpace(customerEmail))
            return OrderErrors.CustomerEmailIsRequired(customerEmail);

        if (string.IsNullOrWhiteSpace(orderNumber))
            return OrderErrors.OrderNumberIsRequired(orderNumber);

        // Автоматически генерируем Slug на основе OrderNumber (например, "ord-2026-876622")
        var slug = GenerateSlug(orderNumber);

        var order = new Order(
            Guid.NewGuid(),
            customerId,
            customerEmail,
            orderNumber,
            slug,
            shippingAddress,
            currency);

        order.AddDomainEvent(
            new OrderCreatedDomainEvent(order.Id, order.CustomerId));

        return Result.Success(order);
    }

    private static string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return input.ToLowerInvariant().Trim()
            .Replace(" ", "-")
            .Replace("_", "-");
    }

    public Result<Money> GetTotalAmount()
    {
        if (!_items.Any())
            return Money.Create(0, Currency);

        Money total = _items[0].TotalPrice;

        for (int i = 1; i < _items.Count; i++)
        {
            var result = total.Add(_items[i].TotalPrice);

            if (result.IsFailure)
                return Result.Failure<Money>(result.Error);

            total = result.Value;
        }

        return total;
    }

    public Result AddItem(
    Guid productId,
    string productName,
    string sku,
    Money originUnitPrice,
    Money unitPrice,
    int quantity)
    {
        if (Status != OrderStatus.Draft)
            return OrderErrors.OrderCannotBeModified(Status);

        var existing = _items.FirstOrDefault(
            x => x.ProductId == productId);

        if (existing is not null)
        {
            var result = existing.AddQuantity(quantity);

            if (result.IsFailure)
                return result.Error;

            UpdatedAtUtc = DateTime.UtcNow;
            return Result.Success();
        }

        var itemResult = OrderItem.Create(
            productId,
            productName,
            sku,
            originUnitPrice,
            unitPrice,
            quantity);

        if (itemResult.IsFailure)
            return itemResult.Error;

        _items.Add(itemResult.Value);
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result RemoveOrderItem(Guid orderItemId)
    {
        if (Status != OrderStatus.Draft)
        {
            return OrderErrors.OrderCannotBeModified(Status);
        }

        var item = _items.FirstOrDefault(x => x.Id == orderItemId);

        if (item is null)
        {
            return OrderErrors.OrderItemNotFound(orderItemId);
        }

        _items.Remove(item);

        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new OrderItemRemovedDomainEvent(
            Id,
            item.Id,
            item.ProductId));

        return Result.Success();
    }

    public Result ChangeStatus(OrderStatus newStatus)
    {
        if (Status == newStatus)
        {
            return Result.Success();
        }

        if (!IsValidTransition(Status, newStatus))
        {
            return OrderErrors.InvalidStatusTransition(Status, newStatus);
        }

        var oldStatus = Status;

        Status = newStatus;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new OrderStatusChangedDomainEvent(
            Id,
            oldStatus,
            newStatus));

        return Result.Success();
    }

    private static bool IsValidTransition(OrderStatus current, OrderStatus next)
    {
        return current switch
        {
            OrderStatus.Draft => next is OrderStatus.Pending or OrderStatus.Cancelled,
            OrderStatus.Pending => next is OrderStatus.Paid or OrderStatus.Cancelled,
            OrderStatus.Paid => next is OrderStatus.Processing,
            OrderStatus.Processing => next is OrderStatus.Shipped,
            OrderStatus.Shipped => next is OrderStatus.Delivered,
            OrderStatus.Delivered => next is OrderStatus.Refunded,
            OrderStatus.Cancelled => false,
            OrderStatus.Refunded => false,
            _ => false
        };
    }

    public Result ForceChangeStatus(OrderStatus newStatus, Guid changeBy, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        if (Status == newStatus)
        {
            return Result.Success();
        }
        var oldStatus = Status;
        Status = newStatus;
        UpdatedAtUtc = DateTime.UtcNow;
        AddDomainEvent(new OrderStatusForceChangedDomainEvent(
            Id,
            oldStatus,
            newStatus,
            changeBy,
            reason
        ));
        return Result.Success();
    }

    public Result Submit()
    {
        if (Status != OrderStatus.Draft)
            return OrderErrors.InvalidStatusTransition(Status, OrderStatus.Pending);

        if (!_items.Any())
            return OrderErrors.EmptyOrder();

        if (ShippingAddress is null)
            return OrderErrors.ShippingAddressRequired();

        Status = OrderStatus.Pending;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == OrderStatus.Delivered)
            return OrderErrors.OrderAlreadyDelivered();
        if (Status == OrderStatus.Cancelled)
            return OrderErrors.OrderAlreadyCanceled();

        Status = OrderStatus.Cancelled;
        UpdatedAtUtc = DateTime.UtcNow;
        AddDomainEvent(new OrderCancelledDomainEvent(Id));
        return Result.Success();
    }

    public Result UpdateShippingAddress(OrderAddress shippingAddress)
    {
        ArgumentNullException.ThrowIfNull(shippingAddress);
        ShippingAddress = shippingAddress;
        UpdatedAtUtc = DateTime.UtcNow;
        AddDomainEvent(new OrderShippingAddressUpdatedDomainEvent(Id));
        return Result.Success();
    }

    public Result ChangeItemQuantity(Guid orderItemId, int quantity)
    {
        if (Status != OrderStatus.Draft)
        {
            return OrderErrors.OrderCannotBeModified(Status);
        }
        if (quantity <= 0)
        {
            return OrderErrors.NegativeQuantity;
        }
        var item = _items.FirstOrDefault(x => x.Id == orderItemId);
        if (item is null)
        {
            return OrderErrors.OrderItemNotFound(orderItemId);
        }
        item.ChangeQuantity(quantity);
        UpdatedAtUtc = DateTime.UtcNow;
        return Result.Success();
    }
    public Result SetExchangeRateSnapshot(
      decimal exchangeRate,
      DateOnly exchangeRateDate)
    {


        if (Status != OrderStatus.Draft)
            return OrderErrors.OrderCannotBeModified(Status);

        if (exchangeRate <= 0)
            return OrderErrors.InvalidExchangeRate;

        if (ExchangeRate.HasValue || ExchangeRateDate.HasValue)
        {
            return Error.Failure(
                "Order.ExchangeRateAlreadySet",
                "Exchange rate snapshot has already been set.");
        }

        var now = DateTime.UtcNow;

        ExchangeRate = exchangeRate;
        ExchangeRateDate = exchangeRateDate;
        ExchangeRateCapturedAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }
    #endregion
}