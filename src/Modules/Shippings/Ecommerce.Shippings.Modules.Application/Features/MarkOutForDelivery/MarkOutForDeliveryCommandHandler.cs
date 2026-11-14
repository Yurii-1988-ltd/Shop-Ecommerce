

namespace Ecommerce.Shippings.Modules.Application.Features.MarkOutForDelivery;

internal sealed class MarkOutForDeliveryCommandHandler(IShippingRepository repository, IShipmentTrackingEventRepository trackingRepository,
        IUnitOfWork unitOfWork) : ICommandHandler<MarkOutForDeliveryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(MarkOutForDeliveryCommand request, CancellationToken cancellationToken)
    {

        if (request.ShippingId == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        var shipping = await repository.GetByIdAsync(
            request.ShippingId,
            cancellationToken);

        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.ShippingId);
        var result = shipping.MarkOutForDelivery();
        if (result.IsFailure)
            return result.Error;

        var trackingEvent = shipping.CreateTrackingEvent(
            "Shipment is out for delivery.");

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
