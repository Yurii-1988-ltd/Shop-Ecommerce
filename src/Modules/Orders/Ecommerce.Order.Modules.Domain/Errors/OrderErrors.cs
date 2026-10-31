using Ecommerce.Domain.Domain;
using Ecommerce.Order.Modules.Domain.Enums;

namespace Ecommerce.Order.Modules.Domain.Errors;

public static class OrderErrors
{
    public static Error GuidEmpty =>
        new("Empty.Guid", "Guid is Empty", ErrorType.Validation);

    public static Error InvalidPrice =>
        new("Negative.Price", "Price must be greater than zero", ErrorType.Validation);

    public static Error NegativeQuantity =>
        new("Negative.Quantity", "Quantity must be greater than zero", ErrorType.Validation);

    public static Error SkuIsRequired =>
        new("Sku.Required", "Sku is required", ErrorType.Validation);

    public static Error SlugIsRequired =>
        new("Slug.Required", "Slug is required", ErrorType.Validation); 

    public static Error InvalidProductId =>
        new("Invalid.ProductId", "Product ID is invalid", ErrorType.Validation);

    public static Error NameIsRequired =>
        new("Name.Required", "Name is required", ErrorType.Validation);

    public static Error NotFound(Guid id) =>
        new("Order.NotFound", $"Order with ID {id} was not found", ErrorType.NotFound);

    // Перегрузка для поиска по string (например, по Slug, раз мы его используем)
    public static Error NotFound(string identifier) =>
        new("Order.NotFound", $"Order with identifier '{identifier}' was not found", ErrorType.NotFound);

    public static Error AlreadyExist(Guid id) =>
        new("Order.AlreadyExists", $"Order with ID {id} already exists", ErrorType.Validation);

    public static Error OrderIsNotEditable =>
        new("Order.IsNotEditable", "Order is not editable", ErrorType.Validation);

    public static Error InvalidStatusTransition(OrderStatus current, OrderStatus next) =>
        new("Order.InvalidStatusTransition", $"Cannot change status from {current} to {next}", ErrorType.Conflict);

    public static Error OrderAlreadyDelivered() =>
        new("Order.AlreadyDelivered", "Order is already delivered", ErrorType.Conflict);

    public static Error OrderAlreadyCanceled() =>
        new("Order.AlreadyCanceled", "Order is already canceled", ErrorType.Conflict);

    public static Error EmptyOrder() =>
        new("Order.Empty", "Order cannot be empty", ErrorType.Validation);

    public static Error ShippingAddressRequired() =>
        new("Order.ShippingAddressRequired", "Shipping address is required", ErrorType.Validation);

    public static Error OrderCannotBeModified(OrderStatus status) =>
        new("Order.CannotBeModified", $"Order with status {status} cannot be modified", ErrorType.Failure);

    public static Error OrderItemNotFound(Guid id) =>
        new("OrderItem.NotFound", $"Order item with ID {id} was not found", ErrorType.NotFound);

    public static Error OrderNumberIsRequired(string? orderNumber = null) =>
        new("Order.NumberIsRequired", "Order number is required", ErrorType.Validation);

    public static Error CustomerEmailIsRequired(string? customerEmail = null) =>
        new("Order.CustomerEmailIsRequired", "Customer email is required", ErrorType.Validation);

    public static Error AccessDenied =>
        new("Order.AccessDenied", "You do not have access to this order", ErrorType.Validation);
    public static Error InvalidExchangeRate =>
        new("Invalid.ExchangeRate", "Exchange is invalid", ErrorType.Validation);

    public static Error ShipmentIdRequired =>
        new("Shipping.ShipmentIdRequired", "Shipment ID is required", ErrorType.Validation);

    public static Error StatusIsRequired =>
        new("Shipping.StatusIsRequired", "Tracking status is required", ErrorType.Validation);

    public static Error DescriptionIsRequired =>
        new("Shipping.DescriptionIsRequired", "Tracking description is required", ErrorType.Validation);

    public static Error InvalidOccurredAt =>
        new("Shipping.InvalidOccurredAt", "Tracking event date cannot be in the future", ErrorType.Validation);
    public static Error ShipmentIdIsRequired =>
    new("Shipping.ShipmentIdIsRequired", "Shipment ID is required", ErrorType.Validation);

    

}