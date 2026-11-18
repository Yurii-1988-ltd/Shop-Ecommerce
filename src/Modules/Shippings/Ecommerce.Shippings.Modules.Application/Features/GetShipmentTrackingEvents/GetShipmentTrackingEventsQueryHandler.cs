


namespace Ecommerce.Shippings.Modules.Application.Features.GetShipmentTrackingEvents;

internal sealed class GetShipmentTrackingEventsQueryHandler(IShipmentTrackingEventRepository repository) : IQueryHandler<GetShipmentTrackingEventsQuery, IReadOnlyCollection<ShipmentTrackingEventResponse>>
{
    public async Task<Result<IReadOnlyCollection<ShipmentTrackingEventResponse>>> Handle(
     GetShipmentTrackingEventsQuery request,
     CancellationToken cancellationToken)
    {
        if (request.ShipmentId == Guid.Empty)
            return ShippingErrors.ShipmentIdIsRequired;

        var events = await repository.GetByShipmentIdAsync(
            request.ShipmentId,
            cancellationToken);

        var response = events
            .Select(x => new ShipmentTrackingEventResponse(
                x.Id,
                x.ShipmentId,
                x.Status.ToString(),
                x.Description,
                x.OccurredAtUtc))
            .ToList();

        return response;
    }
}
