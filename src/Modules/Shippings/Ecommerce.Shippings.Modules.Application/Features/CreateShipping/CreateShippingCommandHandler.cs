using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Application.Abstractions;
using Ecommerce.Shippings.Modules.Domain.Entities;
using Ecommerce.Shippings.Modules.Domain.Errors;
using Ecommerce.Shippings.Modules.Domain.Repositories;

namespace Ecommerce.Shippings.Modules.Application.Features.CreateShipping;

public sealed class CreateShippingCommandHandler(
    IShippingRepository shippingRepository,
    IShippingMethodRepository shippingMethodRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid>> Handle(
        CreateShippingCommand request,
        CancellationToken cancellationToken)
    {

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

        var shippingResult = Shipping.Create(
            Guid.NewGuid(),
            request.OrderId,
            shippingMethod.Id,
            shippingMethod.Name,
            shippingMethod.Price,
            request.RecipientName,
            request.RecipientPhone,
            request.Country,
            request.City,
            request.Street,
            request.Building,
            request.PostalCode,
            request.DeliveryPoint,
            request.Notes);

        if (shippingResult.IsFailure)
            return shippingResult.Error;

        await shippingRepository.AddAsync(
            shippingResult.Value,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return shippingResult.Value.Id;
    }
}