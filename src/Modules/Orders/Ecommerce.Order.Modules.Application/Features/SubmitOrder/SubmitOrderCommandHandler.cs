
namespace Ecommerce.Order.Modules.Application.Features.SubmitOrder;

internal sealed class SubmitOrderCommandHandler(
    IOrderRepository repository)
    : ICommandHandler<SubmitOrderCommand>
{
    public async Task<Result> Handle(
        SubmitOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
            return OrderErrors.NotFound(request.OrderId);

        var result = order.Submit();

        if (result.IsFailure)
            return result;

        return await repository.UpdateAsync(
            order,
            cancellationToken);
    }
}
