using Ecommerce.Application.CQRS;
using Ecommerce.Domain.Domain;
using Ecommerce.Shippings.Modules.Application.Abstractions;
using Ecommerce.Shippings.Modules.Domain.Errors;
using Ecommerce.Shippings.Modules.Domain.Repositories;

namespace Ecommerce.Shippings.Modules.Application.Features.DeleteShippingMethod;

internal sealed class DeleteShippingMethodCommandHandler(
    IShippingMethodRepository repository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteShippingMethodCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        DeleteShippingMethodCommand request,
        CancellationToken cancellationToken)
    {
        var shippingMethod = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (shippingMethod is null)
            return ShippingErrors.ShippingMethodNotFound;

        var isUsed = await repository.IsUsedAsync(
            request.Id,
            cancellationToken);

        if (isUsed)
            return ShippingErrors.ShippingMethodInUse;

        repository.Remove(shippingMethod);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return shippingMethod.Id;
    }
}