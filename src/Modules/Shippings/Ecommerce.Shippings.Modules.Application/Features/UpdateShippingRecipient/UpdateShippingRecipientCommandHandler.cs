
namespace Ecommerce.Shippings.Modules.Application.Features.UpdateShippingRecipient;

internal sealed class UpdateShippingRecipientCommandHandler(
    IShippingRepository repository,
    IShipmentTrackingEventRepository trackingRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateShippingRecipientCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        UpdateShippingRecipientCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ShippingId == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        var shipping = await repository.GetByIdAsync(
            request.ShippingId,
            cancellationToken);

        if (shipping is null)
            return ShippingErrors.ShippingNotFound(request.ShippingId);

        var detailsResult = RecipientDetails.Create(
            request.RecipientName,
            request.RecipientPhone,
            request.Country,
            request.City,
            request.Street,
            request.Building,
            request.PostalCode,
            request.DeliveryPoint);

        if (detailsResult.IsFailure)
            return detailsResult.Error;

        var result = shipping.UpdateRecipientDetails(
            detailsResult.Value);

        if (result.IsFailure)
            return result.Error;

        var trackingEvent = shipping.CreateTrackingEvent(
            "Recipient and delivery address updated.");

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
