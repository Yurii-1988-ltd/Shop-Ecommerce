using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Domain.ValueObjects;
using Ecommerce.Shippings.Modules.Application.Abstractions;
using Ecommerce.Shippings.Modules.Domain.Entities;
using Ecommerce.Shippings.Modules.Domain.Errors;
using Ecommerce.Shippings.Modules.Domain.Repositories;

namespace Ecommerce.Shippings.Modules.Application.Features.CreateShippingMethod;

public sealed class CreateShippingMethodCommandHandler(IShippingMethodRepository repository,
            IUnitOfWork  unitOfWork) : ICommandHandler<CreateShippingMethodCommand,Guid>
{
    public async Task<Result<Guid>> Handle(CreateShippingMethodCommand request, CancellationToken cancellationToken)
    {
        var moneyResult = Money.Create(request.Price,request.Currency);
        if (moneyResult.IsFailure)
            return moneyResult.Error;
        if (await repository.ExistsByCodeAsync(
            request.Code,
            cancellationToken: cancellationToken))
        {
            return ShippingErrors.ShippingMethodCodeAlreadyExists;
        }

        var shippingMethod = ShippingMethod.Create(
            Guid.NewGuid(),
            request.Name,
            request.Code,
            request.Description,
            moneyResult.Value);
        if (shippingMethod.IsFailure)
            return shippingMethod.Error;
        await repository.AddAsync(shippingMethod.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return shippingMethod.Value.Id;


    }
}