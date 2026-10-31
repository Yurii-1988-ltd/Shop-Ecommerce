
using Ecommerce.Domain.Domain;
using Ecommerce.Domain.ValueObjects;
using Ecommerce.Shippings.Modules.Domain.Enums;
using Ecommerce.Shippings.Modules.Domain.Errors;

namespace Ecommerce.Shippings.Modules.Domain.Entities;

public sealed class Shipping : Entity
{
    public Guid OrderId { get; private set; }
    public Guid ShippingMethodId { get; private set; }
    public string ShippingMethodName { get; private set; } = string.Empty;
    public Money ShippingCost { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public ShippingStatus Status { get; private set; }

    public string RecipientName { get; private set; } = string.Empty;
    public string RecipientPhone { get; private set; } = string.Empty;

    public string Country { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string? Street { get; private set; }
    public string? Building { get; private set; }
    public string? PostalCode { get; private set; }
    public string? DeliveryPoint { get; private set; }

    public string? TrackingNumber { get; private set; }
    public string? Notes { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ShippedAtUtc { get; private set; }
    public DateTime? DeliveredAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private Shipping()
    {
    }

    private Shipping(
        Guid id,
        Guid orderId,
        Guid shippingMethodId,
        string shippingMethodName,
        Money shippingCost,
        string recipientName,
        string recipientPhone,
        string country,
        string city,
        string? street,
        string? building,
        string? postalCode,
        string? deliveryPoint,
        string? notes)
    {
        Id = id;
        OrderId = orderId;
        ShippingMethodId = shippingMethodId;
        ShippingMethodName = shippingMethodName;
        ShippingCost = shippingCost;
        RecipientName = recipientName;
        RecipientPhone = recipientPhone;
        Country = country;
        City = city;
        Street = street;
        Building = building;
        PostalCode = postalCode;
        DeliveryPoint = deliveryPoint;
        Notes = notes;

        Status = ShippingStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Result<Shipping> Create(
        Guid id,
        Guid orderId,
        Guid shippingMethodId,
        string shippingMethodName,
        Money shippingCost,
        string currency,
        string recipientName,
        string recipientPhone,
        string country,
        string city,
        string? street = null,
        string? building = null,
        string? postalCode = null,
        string? deliveryPoint = null,
        string? notes = null)
    {
        if (id == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        if (orderId == Guid.Empty)
            return ShippingErrors.OrderIdIsRequired;

        if (shippingMethodId == Guid.Empty)
            return ShippingErrors.ShippingMethodIdIsRequired;

        if (string.IsNullOrWhiteSpace(shippingMethodName))
            return ShippingErrors.NameIsRequired;

        if (shippingCost is null)
            return ShippingErrors.InvalidPrice;

        if (string.IsNullOrWhiteSpace(currency))
            return ShippingErrors.CurrencyIsRequired;

        if (string.IsNullOrWhiteSpace(recipientName))
            return ShippingErrors.RecipientNameIsRequired;

        if (string.IsNullOrWhiteSpace(recipientPhone))
            return ShippingErrors.RecipientPhoneIsRequired;

        if (string.IsNullOrWhiteSpace(country))
            return ShippingErrors.CountryIsRequired;

        if (string.IsNullOrWhiteSpace(city))
            return ShippingErrors.CityIsRequired;

        return new Shipping(
            id,
            orderId,
            shippingMethodId,
            shippingMethodName.Trim(),
            shippingCost,
            recipientName.Trim(),
            recipientPhone.Trim(),
            country.Trim(),
            city.Trim(),
            street?.Trim(),
            building?.Trim(),
            postalCode?.Trim(),
            deliveryPoint?.Trim(),
            notes?.Trim());
    }

    public Result MarkAsShipped()
    {
        if (Status == ShippingStatus.Delivered)
            return ShippingErrors.ShippingAlreadyDelivered;

        if (Status == ShippingStatus.Cancelled)
            return ShippingErrors.ShippingAlreadyCancelled;

        Status = ShippingStatus.Shipped;
        ShippedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result MarkAsDelivered()
    {
        if (Status == ShippingStatus.Delivered)
            return ShippingErrors.ShippingAlreadyDelivered;

        if (Status == ShippingStatus.Cancelled)
            return ShippingErrors.ShippingAlreadyCancelled;

        Status = ShippingStatus.Delivered;
        DeliveredAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == ShippingStatus.Delivered)
            return ShippingErrors.ShippingAlreadyDelivered;

        if (Status == ShippingStatus.Cancelled)
            return ShippingErrors.ShippingAlreadyCancelled;

        Status = ShippingStatus.Cancelled;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateTrackingNumber(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            return ShippingErrors.TrackingNumberIsRequired;

        if (Status == ShippingStatus.Delivered)
            return ShippingErrors.ShippingAlreadyDelivered;

        if (Status == ShippingStatus.Cancelled)
            return ShippingErrors.ShippingAlreadyCancelled;

        TrackingNumber = trackingNumber.Trim();
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }
}
