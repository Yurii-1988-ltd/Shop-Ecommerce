

namespace Ecommerce.Shippings.Modules.Application.Features.MarkAsShipped;

internal sealed class MarkAsShippedCommandHandler(IShippingRepository repository,
    IShipmentTrackingEventRepository trackingRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<MarkAsShippedCommand, Guid>
{
    public async Task<Result<Guid>> Handle(MarkAsShippedCommand request, CancellationToken cancellationToken)
    {
        if (request.ShippingId == Guid.Empty)
            return ShippingErrors.GuidEmpty;
        var shipping = await repository.GetByIdAsync(
            request.ShippingId,
            cancellationToken);

        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.ShippingId);

        var result = shipping.MarkAsShipped();

        if (result.IsFailure)
            return result.Error;

        var trackingEvent = shipping.CreateTrackingEvent(
            "Shipment has been shipped.");

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
