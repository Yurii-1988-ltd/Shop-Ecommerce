

namespace Ecommerce.Shippings.Modules.Application.Features.MarkInTransit
{
    internal sealed class MarkInTransitCommandHandler(IShippingRepository repository, IShipmentTrackingEventRepository trackingRepository,
        IUnitOfWork unitOfWork) : ICommandHandler<MarkInTransitCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(MarkInTransitCommand request, CancellationToken cancellationToken)
        {
            if (request.ShippingId == Guid.Empty)
                return ShippingErrors.GuidEmpty;

            var shipping = await repository.GetByIdAsync(
                request.ShippingId,
                cancellationToken);

            if (shipping is null)
                return ShippingErrors.ShippingNotFound(request.ShippingId);
            var result = shipping.MarkInTransit();
            if (result.IsFailure)
                return result.Error;

            var trackingEvent = shipping.CreateTrackingEvent(
                "Shipment is in transit.");

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
}
