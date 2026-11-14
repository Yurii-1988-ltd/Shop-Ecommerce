
using Ecommerce.Domain.Domain;
using Ecommerce.Domain.ValueObjects;
using Ecommerce.Shipings.Modules.Domain.Entities;
using Ecommerce.Shipings.Modules.Domain.Enums;
using Ecommerce.Shippings.Modules.Domain.Errors;

namespace Ecommerce.Shippings.Modules.Domain.Entities;

public sealed class Shipping : Entity
{
    #region Properties

    public Guid OrderId { get; private set; }
    public Guid ShippingMethodId { get; private set; }
    public string ShippingMethodName { get; private set; } = string.Empty;
    public Money ShippingCost { get; private set; } = null!;

    public ShippingStatus Status { get; private set; }

    public RecipientDetails RecipientDetails { get; private set; } = null!;

    public string? TrackingNumber { get; private set; }
    public string? Notes { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ShippedAtUtc { get; private set; }
    public DateTime? DeliveredAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    #endregion

    #region Constructors

    private Shipping()
    {
    }

    private Shipping(
        Guid id,
        Guid orderId,
        Guid shippingMethodId,
        string shippingMethodName,
        Money shippingCost,
        RecipientDetails recipientDetails,
        string? notes)
    {
        Id = id;
        OrderId = orderId;
        ShippingMethodId = shippingMethodId;
        ShippingMethodName = shippingMethodName;
        ShippingCost = shippingCost;
        RecipientDetails = recipientDetails;
        Notes = notes;

        Status = ShippingStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    #endregion

    #region Factory methods

    public static Result<Shipping> Create(
        Guid id,
        Guid orderId,
        Guid shippingMethodId,
        string shippingMethodName,
        Money shippingCost,
        RecipientDetails recipientDetails,
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

        if (shippingCost is null || shippingCost.Amount < 0)
            return ShippingErrors.InvalidPrice;

        if (string.IsNullOrWhiteSpace(shippingCost.Currency) ||
            shippingCost.Currency.Trim().Length != 3)
            return ShippingErrors.CurrencyIsRequired;

        if (recipientDetails is null)
            return ShippingErrors.RecipientDetailsIsRequired;

        return new Shipping(
            id,
            orderId,
            shippingMethodId,
            shippingMethodName.Trim(),
            shippingCost,
            recipientDetails,
            notes?.Trim());
    }

    #endregion

    #region Status methods

    public Result MarkAsShipped()
    {
        if (Status == ShippingStatus.Delivered)
            return ShippingErrors.ShippingAlreadyDelivered;

        if (Status == ShippingStatus.Cancelled)
            return ShippingErrors.ShippingAlreadyCancelled;

        if (Status == ShippingStatus.Shipped)
            return ShippingErrors.ShippingAlreadyShipped;

        if (Status is not (
            ShippingStatus.Pending or
            ShippingStatus.Processing or
            ShippingStatus.ReadyForShipment))
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                ShippingStatus.Shipped.ToString());
        }

        var now = DateTime.UtcNow;

        Status = ShippingStatus.Shipped;
        ShippedAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    public Result MarkAsDelivered()
    {
        if (Status == ShippingStatus.Delivered)
            return ShippingErrors.ShippingAlreadyDelivered;

        if (Status == ShippingStatus.Cancelled)
            return ShippingErrors.ShippingAlreadyCancelled;

        if (Status is not (
            ShippingStatus.Shipped or
            ShippingStatus.InTransit or
            ShippingStatus.OutForDelivery))
        {
            return ShippingErrors.ShippingMustBeShippedFirst;
        }

        var now = DateTime.UtcNow;

        Status = ShippingStatus.Delivered;
        DeliveredAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is not (
            ShippingStatus.Pending or
            ShippingStatus.Processing or
            ShippingStatus.ReadyForShipment))
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                ShippingStatus.Cancelled.ToString());
        }

        Status = ShippingStatus.Cancelled;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result StartProcessing()
    {
        if (Status != ShippingStatus.Pending)
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                ShippingStatus.Processing.ToString());
        }

        Status = ShippingStatus.Processing;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result MarkAsReadyForShipment()
    {
        if (Status != ShippingStatus.Processing)
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                ShippingStatus.ReadyForShipment.ToString());
        }

        Status = ShippingStatus.ReadyForShipment;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result MarkInTransit()
    {
        if (Status != ShippingStatus.Shipped)
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                ShippingStatus.InTransit.ToString());
        }

        Status = ShippingStatus.InTransit;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result MarkOutForDelivery()
    {
        if (Status != ShippingStatus.InTransit)
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                ShippingStatus.OutForDelivery.ToString());
        }

        Status = ShippingStatus.OutForDelivery;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result MarkAsFailed()
    {
        if (Status is not (
            ShippingStatus.Shipped or
            ShippingStatus.InTransit or
            ShippingStatus.OutForDelivery))
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                ShippingStatus.Failed.ToString());
        }

        Status = ShippingStatus.Failed;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result MarkAsReturned()
    {
        if (Status is not (
            ShippingStatus.Failed or
            ShippingStatus.Delivered or
            ShippingStatus.InTransit or
            ShippingStatus.OutForDelivery))
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                ShippingStatus.Returned.ToString());
        }

        Status = ShippingStatus.Returned;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    #endregion

    #region Update methods

    public Result UpdateTrackingNumber(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            return ShippingErrors.TrackingNumberIsRequired;

        if (Status is not (
            ShippingStatus.Pending or
            ShippingStatus.Processing or
            ShippingStatus.ReadyForShipment))
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                "UpdateTrackingNumber");
        }

        TrackingNumber = trackingNumber.Trim();
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateRecipientDetails(RecipientDetails recipientDetails)
    {
        if (recipientDetails is null)
            return ShippingErrors.RecipientDetailsIsRequired;

        if (Status is not (
            ShippingStatus.Pending or
            ShippingStatus.Processing or
            ShippingStatus.ReadyForShipment))
        {
            return ShippingErrors.InvalidStatusTransition(
                Status.ToString(),
                "UpdateRecipientDetails");
        }

        RecipientDetails = recipientDetails;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    #endregion

    #region Tracking

    public Result<ShipmentTrackingEvent> CreateTrackingEvent(
        string description,
        string? location = null)
    {
        return ShipmentTrackingEvent.Create(
            Guid.NewGuid(),
            Id,
            Status,
            description,
            location,
            DateTime.UtcNow);
    }
    public Result ChangeStatus(ShippingStatus newStatus, string? trackingNumber = null)
    {
        if (!string.IsNullOrWhiteSpace(trackingNumber))
        {
            var trackingResult = UpdateTrackingNumber(trackingNumber);
            if (trackingResult.IsFailure) return trackingResult.Error;
        }

        return newStatus switch
        {
            ShippingStatus.Processing => StartProcessing(),
            ShippingStatus.ReadyForShipment => MarkAsReadyForShipment(),
            ShippingStatus.Shipped => MarkAsShipped(),
            ShippingStatus.InTransit => MarkInTransit(),
            ShippingStatus.OutForDelivery => MarkOutForDelivery(),
            ShippingStatus.Delivered => MarkAsDelivered(),
            ShippingStatus.Failed => MarkAsFailed(),
            ShippingStatus.Returned => MarkAsReturned(),
            ShippingStatus.Cancelled => Cancel(),
            _ => ShippingErrors.InvalidStatusTransition(Status.ToString(), newStatus.ToString())
        };
    }
    public Result UpdateShippingStatusInfo(string shippingStatus, string? trackingNumber = null)
    {
        // 1. Сохраняем трекинг-номер, если он передан
        if (!string.IsNullOrWhiteSpace(trackingNumber))
        {
            var trackingResult = UpdateTrackingNumber(trackingNumber);
            if (trackingResult.IsFailure)
                return trackingResult.Error;
        }

        // 2. Пытаемся конвертировать строковый статус в доменный ShippingStatus
        if (Enum.TryParse<ShippingStatus>(shippingStatus, ignoreCase: true, out var newShippingStatus))
        {
            var changeStatusResult = ChangeStatus(newShippingStatus);
            if (changeStatusResult.IsFailure)
            {
                return changeStatusResult.Error;
            }
        }
        else
        {
            UpdatedAtUtc = DateTime.UtcNow;
        }

        return Result.Success();
    }

    #endregion
}
