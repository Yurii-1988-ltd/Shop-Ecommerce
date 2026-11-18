
using MassTransit;

namespace Ecommerce.Shippings.Modules.Application.Features.UpdateShippingStatus;

internal sealed class UpdateShippingStatusCommandHandler(
    IShippingRepository shippingRepository,
    IShipmentTrackingEventRepository trackingEventRepository,
    IPublishEndpoint publishEndpoint,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateShippingStatusCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        UpdateShippingStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ShippingId == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        var shipping = await shippingRepository.GetByIdAsync(
            request.ShippingId,
            cancellationToken);

        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.ShippingId);

        var updateResult = shipping.UpdateShippingStatusInfo(request.Status);

        if (updateResult.IsFailure)
            return updateResult.Error;

        // Create tracking event
        var trackingResult = shipping.CreateTrackingEvent(
            $"Shipping status changed to {shipping.Status}");

        if (trackingResult.IsFailure)
            return trackingResult.Error;

        // Add tracking event to repository
        await trackingEventRepository.AddAsync(
            trackingResult.Value,
            cancellationToken);

        // Save shipping and tracking event together
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish integration event
        await publishEndpoint.Publish(
            new ShippingStatusChangedIntegrationEvent(
                shipping.Id,
                shipping.OrderId,
                shipping.Status.ToString(),
                shipping.TrackingNumber,
                shipping.UpdatedAtUtc ?? DateTime.UtcNow
            ),
            cancellationToken);

        return Result<Guid>.Success(shipping.Id);
    }
}
