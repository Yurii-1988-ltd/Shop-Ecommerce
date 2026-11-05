using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Application.Abstractions;
using Ecommerce.Shippings.Modules.Domain.Errors;
using Ecommerce.Shippings.Modules.Domain.Repositories;

namespace Ecommerce.Shippings.Modules.Application.Features.ActivateShippingMethod;

internal sealed class ActivateShippingMethodCommandHandler(IShippingMethodRepository repository, IUnitOfWork unitOfWork) : ICommandHandler<ActivateShippingMethodCommand,Guid>
{
    public async Task<Result<Guid>> Handle(ActivateShippingMethodCommand request, CancellationToken cancellationToken)
    {
        
        var shippingMethod = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (shippingMethod is null)
            return ShippingErrors.ShippingMethodNotFound;
        var result = shippingMethod.Activate();
        if (result.IsFailure)
            return result.Error;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return shippingMethod.Id;
        
    }
}