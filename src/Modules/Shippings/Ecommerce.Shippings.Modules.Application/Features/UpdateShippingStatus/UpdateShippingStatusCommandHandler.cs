


using MassTransit;

namespace Ecommerce.Shippings.Modules.Application.Features.UpdateShippingStatus;

internal sealed class UpdateShippingStatusCommandHandler(
    IShippingRepository _shippingRepository,
    IPublishEndpoint publishEndpoint,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateShippingStatusCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateShippingStatusCommand request, CancellationToken cancellationToken)
    {
        if (request.ShippingId == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        var shipping = await _shippingRepository.GetByIdAsync(request.ShippingId, cancellationToken);
        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.ShippingId);

        var updateResult = shipping.UpdateShippingStatusInfo(request.Status);
        if (updateResult.IsFailure)
        {
            return updateResult.Error;
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new ShippingStatusChangedIntegrationEvent(
            shipping.Id,
            shipping.OrderId,
            shipping.Status.ToString(),
            shipping.TrackingNumber,
            shipping.UpdatedAtUtc ?? DateTime.UtcNow
        ), cancellationToken);

        return Result<Guid>.Success(shipping.Id);
    }
}