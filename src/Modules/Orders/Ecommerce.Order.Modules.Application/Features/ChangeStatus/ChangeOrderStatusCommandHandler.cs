using Ecommerce.Order.Modules.Domain.Enums;
using Ecommerce.Shared.Contracts.IntegrationEvent;
using MassTransit;

namespace Ecommerce.Order.Modules.Application.Features.ChangeStatus;

internal sealed class ChangeOrderStatusCommandHandler(
    IOrderRepository repository,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<ChangeOrderStatusCommand>
{
    public async Task<Result> Handle(
        ChangeOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (request.OrderId == Guid.Empty)
            return OrderErrors.GuidEmpty;

        var order = await repository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
            return OrderErrors.NotFound(request.OrderId);

        var previousStatus = order.Status;

        var result = order.ChangeStatus(request.OrderStatus);

        if (result.IsFailure)
            return result;

        var updateResult = await repository.UpdateAsync(
            order,
            cancellationToken);

        if (updateResult.IsFailure)
            return updateResult;

        if (previousStatus != OrderStatus.Paid &&
            order.Status == OrderStatus.Paid)
        {
            var integrationEvent = new OrderPaidIntegrationEvent(
                order.Id,
                order.OrderNumber,
                order.CustomerId,
                order.CustomerEmail,
                new OrderAddressIntegrationDto(
                    order.ShippingAddress.FirstName,
                    order.ShippingAddress.LastName,
                    order.ShippingAddress.Country,
                    order.ShippingAddress.City,
                    order.ShippingAddress.Street,
                    order.ShippingAddress.ZipCode
                ),
                order.ShippingMethodId!.Value,
                order.ShippingMethodName!,
                order.ShippingCost!.Amount,
                order.ShippingCost.Currency,
              RecipientPhone:  order.RecipientPhone!,
                order.PaidAtUtc!.Value
            );

            await publishEndpoint.Publish(
                integrationEvent,
                cancellationToken);
        }

        return Result.Success();
    }
}