
namespace Ecommerce.Shippings.Modules.Application.Features.UpdateRecipientDetails;

internal sealed class UpdateRecipientDetailsCommandHandler(
    IShippingRepository repository,
    IShipmentTrackingEventRepository trackingRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateRecipientDetailsCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        UpdateRecipientDetailsCommand request,
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
            "Recipient details have been updated.");

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
