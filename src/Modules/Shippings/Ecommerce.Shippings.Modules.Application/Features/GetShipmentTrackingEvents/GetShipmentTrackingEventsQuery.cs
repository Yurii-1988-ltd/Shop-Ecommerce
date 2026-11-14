

using Ecommerce.Application.CQRS;
using Ecommerce.Shippings.Modules.Application.Dto;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShipmentTrackingEvents;

public sealed record GetShipmentTrackingEventsQuery(Guid ShipmentId) : IQuery<IReadOnlyCollection<ShipmentTrackingEventResponse>>;

