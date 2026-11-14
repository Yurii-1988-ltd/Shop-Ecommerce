

namespace Ecommerce.Shippings.Modules.Application.Features.MarkAsFailed;

internal sealed class MarkAsFailedCommandHandler(IShippingRepository repository,
    IShipmentTrackingEventRepository trackingRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<MarkAsFailedCommand, Guid>
{
    public async Task<Result<Guid>> Handle(MarkAsFailedCommand request, CancellationToken cancellationToken)
    {

        if (request.ShippingId == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        var shipping = await repository.GetByIdAsync(
            request.ShippingId,
            cancellationToken);

        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.ShippingId);
        var result = shipping.MarkAsFailed();
        if (result.IsFailure)
            return result.Error;

        var trackingEvent = shipping.CreateTrackingEvent(
            "Shipment delivery has failed.");

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
