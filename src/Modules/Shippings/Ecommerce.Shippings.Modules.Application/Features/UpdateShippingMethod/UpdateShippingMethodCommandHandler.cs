
namespace Ecommerce.Shippings.Modules.Application.Features.UpdateShippingMethod;

public sealed class UpdateShippingMethodCommandHandler(IShippingMethodRepository repository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateShippingMethodCommand,Guid>
{
    public async Task<Result<Guid>> Handle(UpdateShippingMethodCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            return ShippingErrors.GuidEmpty;
        var moneyResult = Money.Create(request.Price,request.Currency);
        if (moneyResult.IsFailure)
            return moneyResult.Error;

        if (await repository.ExistsByCodeAsync(
            request.Code,
            request.Id,
            cancellationToken))
        {
            return ShippingErrors.ShippingMethodCodeAlreadyExists;
        }
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