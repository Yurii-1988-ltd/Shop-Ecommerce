

namespace Ecommerce.Shippings.Modules.Application.Features.CreateShipping;

internal sealed class CreateShippingCommandHandler(
    IShippingRepository shippingRepository,
    IShippingMethodRepository shippingMethodRepository,
    IShipmentTrackingEventRepository trackingRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateShippingCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateShippingCommand request,
        CancellationToken cancellationToken)
    {
        if (request.OrderId == Guid.Empty)
            return ShippingErrors.OrderIdIsRequired;

        var existingShipping = await shippingRepository.GetByOrderIdAsync(
            request.OrderId,
            cancellationToken);

        if (existingShipping is not null)
            return ShippingErrors.ShippingAlreadyExists(request.OrderId);

        var shippingMethod = await shippingMethodRepository.GetByIdAsync(
            request.ShippingMethodId,
            cancellationToken);

        if (shippingMethod is null)
            return ShippingErrors.ShippingMethodNotFound;

        if (!shippingMethod.IsActive)
            return ShippingErrors.ShippingMethodIsInactive;

        var recipientResult = RecipientDetails.Create(
            request.RecipientName,
            request.RecipientPhone,
            request.Country,
            request.City,
            request.Street,
            request.Building,
            request.PostalCode,
            request.DeliveryPoint);

        if (recipientResult.IsFailure)
            return recipientResult.Error;

        var shippingResult = Shipping.Create(
            Guid.NewGuid(),
            request.OrderId,
            shippingMethod.Id,
            shippingMethod.Name,
            shippingMethod.Price,
            recipientResult.Value,
            request.Notes);

        if (shippingResult.IsFailure)
            return shippingResult.Error;

        var shipping = shippingResult.Value;

        var trackingEvent = shipping.CreateTrackingEvent(
            "Shipment has been created.");

        if (trackingEvent.IsFailure)
            return trackingEvent.Error;

        await shippingRepository.AddAsync(
            shipping,
            cancellationToken);

        await trackingRepository.AddAsync(
            trackingEvent.Value,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return shipping.Id;
    }
}
