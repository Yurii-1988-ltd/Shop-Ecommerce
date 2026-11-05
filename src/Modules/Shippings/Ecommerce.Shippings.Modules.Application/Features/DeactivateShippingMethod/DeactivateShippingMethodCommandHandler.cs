using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Application.Abstractions;
using Ecommerce.Shippings.Modules.Domain.Errors;
using Ecommerce.Shippings.Modules.Domain.Repositories;

namespace Ecommerce.Shippings.Modules.Application.Features.DeactivateShippingMethod;

internal sealed class DeactivateShippingMethodCommandHandler(IShippingMethodRepository repository,IUnitOfWork unitOfWork)
    : ICommandHandler<DeactivateShippingMethodCommand,Guid>
{
    public async Task<Result<Guid>> Handle(DeactivateShippingMethodCommand request, CancellationToken cancellationToken)
    {
        var shippingMethod = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (shippingMethod is null)
            return ShippingErrors.ShippingMethodNotFound;
        var result = shippingMethod.Deactivate();
        if (result.IsFailure)
            return result.Error;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return shippingMethod.Id;
    }
}