using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Domain.ValueObjects;
using Ecommerce.Shippings.Modules.Application.Abstractions;
using Ecommerce.Shippings.Modules.Domain.Errors;
using Ecommerce.Shippings.Modules.Domain.Repositories;

namespace Ecommerce.Shippings.Modules.Application.Features.UpdateShippingMethod;

public sealed class UpdateShippingMethodCommandHandler(IShippingMethodRepository repository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateShippingMethodCommand,Guid>
{
    public async Task<Result<Guid>> Handle(UpdateShippingMethodCommand request, CancellationToken cancellationToken)
    {
        var moneyResult = Money.Create(request.Price,request.Currency);
        if (moneyResult.IsFailure)
            return moneyResult.Error;
        var shippingMethod = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (shippingMethod is null)
            return ShippingErrors.ShippingMethodNotFound;

        var updateResult = shippingMethod.Update(
            request.Name,
            request.Code,
            request.Description,
            moneyResult.Value);
     

        if (updateResult.IsFailure)
            return updateResult.Error;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return shippingMethod.Id;
    }
}