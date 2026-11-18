

using Ecommerce.Shippings.Modules.Application.Features.MarkShippingAsShipped;

internal sealed class MarkShippingAsShippedCommandHandler(
    IShippingRepository repository,
    IShipmentTrackingEventRepository trackingRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<MarkShippingAsShippedCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        MarkShippingAsShippedCommand request,
        CancellationToken cancellationToken)
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