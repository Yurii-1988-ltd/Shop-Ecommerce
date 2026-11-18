using Ecommerce.Domain.Domain;

namespace Ecommerce.Shippings.Modules.Domain.Errors;

public static class ShippingErrors
{
    #region Validation & Required Fields
    public static Error GuidEmpty =>
        new("Shipping.GuidEmpty", "Guid is Empty", ErrorType.Validation);

    public static Error OrderIdIsRequired =>
        new("Shipping.OrderIdIsRequired", "Order ID is required", ErrorType.Validation);

    public static Error ShipmentIdIsRequired =>
        new("Shipping.ShipmentIdIsRequired", "Shipment ID is required", ErrorType.Validation);

    public static Error ShippingMethodIdIsRequired =>
        new("Shipping.ShippingMethodIdIsRequired", "Shipping method ID is required", ErrorType.Validation);

    public static Error NameIsRequired =>
        new("Shipping.NameIsRequired", "Shipping method name is required", ErrorType.Validation);

    public static Error DescriptionIsRequired =>
        new("Shipping.DescriptionIsRequired", "Shipping description is required", ErrorType.Validation);

    public static Error InvalidPrice =>
        new("Shipping.InvalidPrice", "Shipping price cannot be negative", ErrorType.Validation);

    public static Error CurrencyIsRequired =>
        new("Shipping.CurrencyIsRequired", "Currency is required", ErrorType.Validation);

    public static Error StatusIsRequired =>
        new("Shipping.StatusIsRequired", "Tracking status is required", ErrorType.Validation);

    public static Error CodeIsRequired =>
        new("Shipping.CodeIsRequired", "Code is required", ErrorType.Validation);

    public static Error TrackingNumberIsRequired =>
        new("Shipping.TrackingNumberIsRequired", "Tracking number is required", ErrorType.Validation);

    public static Error ShippingAddressRequired =>
        new("Shipping.AddressRequired", "Shipping address is required", ErrorType.Validation);

    public static Error RecipientDetailsIsRequired =>
        new("Shipping.RecipientDetailsIsRequired", "Recipient details are required.", ErrorType.Validation);

    public static Error RecipientNameIsRequired =>
        new("Shipping.RecipientNameIsRequired", "Recipient name is required", ErrorType.Validation);

    public static Error RecipientPhoneIsRequired =>
        new("Shipping.RecipientPhoneIsRequired", "Recipient phone is required", ErrorType.Validation);

    public static Error CountryIsRequired =>
        new("Shipping.CountryIsRequired", "Country is required", ErrorType.Validation);

    public static Error CityIsRequired =>
        new("Shipping.CityIsRequired", "City is required", ErrorType.Validation);
    #endregion

    #region Not Found
    public static Error NotFound(Guid id) =>
        new("Shipping.NotFound", $"Shipping with ID {id} was not found", ErrorType.NotFound);

    public static Error NotFound(string identifier) =>
        new("Shipping.NotFound", $"Shipping method with identifier '{identifier}' was not found", ErrorType.NotFound);

    public static Error ShippingNotFound(Guid orderId) =>
        new("Shipping.NotFoundForOrder", $"Shipping for order ID {orderId} was not found", ErrorType.NotFound);

    public static Error ShippingMethodNotFound =>
        new("Shipping.MethodNotFound", "Shipping method not found", ErrorType.NotFound);
    #endregion

    #region Conflicts & Business Rules
    public static Error AlreadyExist(Guid id) =>
        new("Shipping.AlreadyExists", $"Shipping method with ID {id} already exists", ErrorType.Conflict);

    public static Error ShippingAlreadyExists(Guid orderId) =>
        new("Shipping.AlreadyExists", $"Shipping for order {orderId} already exists", ErrorType.Conflict);

    public static Error ShippingAlreadyShipped =>
        new("Shipping.AlreadyShipped", "Shipping has already been shipped.", ErrorType.Conflict);

    public static Error ShippingAlreadyDelivered =>
        new("Shipping.AlreadyDelivered", "Shipping is already delivered", ErrorType.Conflict);

    public static Error ShippingAlreadyCancelled =>
        new("Shipping.AlreadyCancelled", "Shipping is already cancelled", ErrorType.Conflict);

    public static Error ShippingMustBeShippedFirst =>
        new("Shipping.MustBeShippedFirst", "Shipping must be shipped before it can be delivered.", ErrorType.Conflict);

    public static Error ShippingMethodIsInactive =>
        new("Shipping.MethodIsInactive", "Shipping method is inactive", ErrorType.Validation);

    public static Error ShippingMethodAlreadyActive =>
        new("Shipping.MethodAlreadyActive", "Shipping method is already active", ErrorType.Conflict);

    public static Error ShippingMethodAlreadyInactive =>
        new("Shipping.MethodAlreadyInactive", "Shipping method is already inactive", ErrorType.Conflict);

    public static Error InvalidShippingMethod =>
        new("Shipping.InvalidMethod", "Shipping method is invalid", ErrorType.Validation);

    public static Error ShippingMethodCodeAlreadyExists =>
        new("ShippingMethod.CodeAlreadyExists", "Shipping method with this code already exists.", ErrorType.Conflict);

    public static Error ShippingMethodInUse =>
        new("ShippingMethod.InUse", "Shipping method cannot be deleted because it is used by existing shipments.", ErrorType.Conflict);

    public static Error InvalidStatusTransition(string current, string next) =>
        new("Shipping.InvalidStatusTransition", $"Cannot change shipping status from {current} to {next}", ErrorType.Conflict);
    #endregion
}