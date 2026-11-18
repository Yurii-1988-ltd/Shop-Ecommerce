

namespace Ecommerce.Shippings.Modules.Application.Features.MarkAsReturned;

internal sealed class MarkAsReturnedCommandHandler(IShippingRepository repository,
    IShipmentTrackingEventRepository trackingRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<MarkAsReturnedCommand, Guid>
{
    public async Task<Result<Guid>> Handle(MarkAsReturnedCommand request, CancellationToken cancellationToken)
    {
        if (request.ShippingId == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        var shipping = await repository.GetByIdAsync(
            request.ShippingId,
            cancellationToken);

        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.ShippingId);
        var result = shipping.MarkAsReturned();
        if (result.IsFailure)
            return result.Error;

        var trackingEvent = shipping.CreateTrackingEvent(
            "Shipment has been returned.");

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
