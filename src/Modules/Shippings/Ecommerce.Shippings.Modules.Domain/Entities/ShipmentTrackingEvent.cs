using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Domain.Enums;
using Ecommerce.Shippings.Modules.Domain.Errors;

namespace Ecommerce.Shippings.Modules.Domain.Entities;

public sealed class ShipmentTrackingEvent : Entity
{
    public Guid ShipmentId { get; private set; }
    public ShippingStatus Status { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? Location { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private ShipmentTrackingEvent()
    {
    }

    private ShipmentTrackingEvent(
        Guid id,
        Guid shipmentId,
        ShippingStatus status,
        string description,
        string? location,
        DateTime occurredAt)
    {
        Id = id;
        ShipmentId = shipmentId;
        Status = status;
        Description = description;
        Location = location;
        OccurredAt = occurredAt;
    }

    public static Result<ShipmentTrackingEvent> Create(
        Guid id,
        Guid shipmentId,
        ShippingStatus status, 
        string description,
        string? location,
        DateTime? occurredAt = null)
    {
        if (id == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        if (shipmentId == Guid.Empty)
            return ShippingErrors.ShipmentIdIsRequired;

        // Проверку для енама можно сделать через Enum.IsDefined, если нужно
        if (!Enum.IsDefined(typeof(ShippingStatus), status))
            return ShippingErrors.StatusIsRequired; // или ShippingErrors.InvalidStatus

        if (string.IsNullOrWhiteSpace(description))
            return ShippingErrors.DescriptionIsRequired;

        return new ShipmentTrackingEvent(
            id,
            shipmentId,
            status,
            description.Trim(),
            string.IsNullOrWhiteSpace(location) ? null : location.Trim(),
            occurredAt ?? DateTime.UtcNow);
    }
}