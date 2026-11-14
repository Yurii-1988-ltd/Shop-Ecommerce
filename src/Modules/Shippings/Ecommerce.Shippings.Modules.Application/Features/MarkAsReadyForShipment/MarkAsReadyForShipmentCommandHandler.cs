

namespace Ecommerce.Shippings.Modules.Application.Features.MarkAsReadyForShipment;

internal sealed class MarkAsReadyForShipmentCommandHandler(IShippingRepository repository,
    IShipmentTrackingEventRepository trackingRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<MarkAsReadyForShipmentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(MarkAsReadyForShipmentCommand request, CancellationToken cancellationToken)
    {
        if (request.ShippingId == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        var shipping = await repository.GetByIdAsync(
            request.ShippingId,
            cancellationToken);

        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.ShippingId);
       var result = shipping.MarkAsReadyForShipment();
        if (result.IsFailure)
            return result.Error;

        var trackingEvent = shipping.CreateTrackingEvent(
            "Shipment is ready for dispatch.");

        if (trackingEvent.IsFailure)
            return trackingEvent.Error;

        repository.Update(shipping);

        await trackingRepository.AddAsync(
            trackingEvent.Value,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return shipping.Id;

    }
}
