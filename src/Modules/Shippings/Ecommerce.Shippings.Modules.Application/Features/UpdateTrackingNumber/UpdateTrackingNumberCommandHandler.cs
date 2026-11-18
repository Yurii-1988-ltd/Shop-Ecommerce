

namespace Ecommerce.Shippings.Modules.Application.Features.UpdateTrackingNumber;

internal sealed class UpdateTrackingNumberCommandHandler(
    IShippingRepository repository,
    IShipmentTrackingEventRepository trackingRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateTrackingNumberCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        UpdateTrackingNumberCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ShippingId == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        var shipping = await repository.GetByIdAsync(
            request.ShippingId,
            cancellationToken);

        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.ShippingId);

        var updateResult = shipping.UpdateTrackingNumber(
            request.TrackingNumber);

        if (updateResult.IsFailure)
            return updateResult.Error;

        var trackingEvent = shipping.CreateTrackingEvent(
     $"Tracking number has been updated to {shipping.TrackingNumber}.");

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