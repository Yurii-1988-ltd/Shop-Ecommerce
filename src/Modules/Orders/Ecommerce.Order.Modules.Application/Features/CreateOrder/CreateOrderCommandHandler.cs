using Ecommerce.Application.Abstractions;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.ValueObjects;
using Ecommerce.Currency.Modules.Application.Abstractions;
using Ecommerce.Order.Modules.Domain.Entities;
using Ecommerce.Order.Modules.Domain.ValueObjects;
using Ecommerce.Shared.Contracts.IntegrationEvent;
using Ecommerce.Shared.Contracts.Orders;
using MassTransit;

public sealed class CreateOrderCommandHandler(
    IOrderRepository repository,
    IEntityNumberGenerator orderNumberGenerator,
    ICurrencyConversionService conversionService,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async Task<Result<CreateOrderResponse>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerEmail))
        {
            return Error.Failure(
                "Order.CustomerEmailRequired",
                "Customer email is required.");
        }

        var currency = string.IsNullOrWhiteSpace(request.Currency)
            ? CurrencyConstant.UAH
            : request.Currency.Trim().ToUpperInvariant();

        var orderNumber = await orderNumberGenerator.GenerateAsync(
            NumberPrefixes.Order,
            cancellationToken);

        var shippingAddress = new OrderAddress(
            request.ShippingAddress.FirstName,
            request.ShippingAddress.LastName,
            request.ShippingAddress.Country,
            request.ShippingAddress.City,
            request.ShippingAddress.Street,
            request.ShippingAddress.ZipCode);

        var orderResult = Order.Create(
            request.CustomerId,
            request.CustomerEmail,
            orderNumber,
            shippingAddress,
            currency);

        if (orderResult.IsFailure)
            return orderResult.Error;

        var order = orderResult.Value;

        // Для иностранной валюты фиксируем курс один раз на весь заказ.
        if (currency != CurrencyConstant.UAH && request.Items.Count > 0)
        {
            var firstItem = request.Items.First();

            var originPriceResult = Money.Create(
                firstItem.UnitPrice,
                CurrencyConstant.UAH);

            if (originPriceResult.IsFailure)
                return originPriceResult.Error;

            var conversionResult =
                await conversionService.ConvertWithRateAsync(
                    originPriceResult.Value,
                    currency,
                    cancellationToken);

            if (conversionResult.IsFailure)
                return conversionResult.Error;

            var conversion = conversionResult.Value;

            if (!conversion.ExchangeRateDate.HasValue)
            {
                return Error.Failure(
                    "Currency.ExchangeRateDateRequired",
                    "Exchange rate date is required for a non-UAH order.");
            }

            var snapshotResult = order.SetExchangeRateSnapshot(
                conversion.ExchangeRate,
                conversion.ExchangeRateDate.Value);

            if (snapshotResult.IsFailure)
                return snapshotResult.Error;
        }

        foreach (var item in request.Items)
        {
            var originPriceResult = Money.Create(
                item.UnitPrice,
                CurrencyConstant.UAH);

            if (originPriceResult.IsFailure)
                return originPriceResult.Error;

            var originUnitPrice = originPriceResult.Value;
            Money unitPrice;

            if (currency == CurrencyConstant.UAH)
            {
                unitPrice = originUnitPrice;
            }
            else
            {
                if (!order.ExchangeRate.HasValue)
                {
                    return Error.Failure(
                        "Currency.ExchangeRateRequired",
                        "Exchange rate snapshot is required.");
                }

                // Используем снимок заказа, а не актуальный курс NBU.
                var conversionResult =
                    await conversionService.ConvertAtRateAsync(
                        originUnitPrice,
                        currency,
                        order.ExchangeRate.Value,
                        cancellationToken);

                if (conversionResult.IsFailure)
                    return conversionResult.Error;

                unitPrice = conversionResult.Value;
            }

            var addItemResult = order.AddItem(
                item.ProductId,
                item.ProductName,
                item.Sku,
                originUnitPrice,
                unitPrice,
                item.Quantity);

            if (addItemResult.IsFailure)
                return addItemResult.Error;
        }

        var totalResult = order.GetTotalAmount();

        if (totalResult.IsFailure)
            return totalResult.Error;

        var items = order.Items
            .Select(item => new OrderCreatedItemIntegrationEvent(
                item.ProductId,
                item.ProductName,
                item.SKU,
                item.UnitPrice.Amount,
                item.Quantity))
            .ToList();

        await repository.InsertAsync(
            order,
            cancellationToken);

        await publishEndpoint.Publish(
            new OrderCreatedIntegrationEvent(
                OrderId: order.Id,
                OrderNumber: order.OrderNumber,
                CustomerId: order.CustomerId,
                CustomerEmail: order.CustomerEmail,
                TotalAmount: totalResult.Value.Amount,
                Currency: order.Currency,
                Items: items,
                CreatedAtUtc: order.CreatedAtUtc),
            cancellationToken);

        return new CreateOrderResponse(order.Id, order.OrderNumber);
    }
}