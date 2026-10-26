
using Ecommerce.Currency.Modules.Application.Abstractions;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.ValueObjects;
using Ecommerce.Order.Modules.Application.Features.AddOrderItem;
using Ecommerce.Order.Modules.Domain.Enums;

public sealed class AddOrderItemCommandHandler(
    IOrderRepository orderRepository,
    IProductService productService,
    ICurrencyConversionService conversionService)
    : ICommandHandler<AddOrderItemCommand>
{
    public async Task<Result> Handle(
        AddOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
            return OrderErrors.NotFound(request.OrderId);

        if (order.Status != OrderStatus.Draft)
        {
            return Error.Validation(
                "Order.InvalidStatus",
                "Items can only be added to draft orders.");
        }

        var productResult = await productService.GetProductAsync(
            request.ProductId,
            cancellationToken);

        if (productResult.IsFailure)
            return productResult.Error;

        var product = productResult.Value;
        var originUnitPrice = product.Price;
        var currency = order.Currency.Trim().ToUpperInvariant();

        Money saleUnitPrice;

        if (currency == CurrencyConstant.UAH)
        {
            saleUnitPrice = originUnitPrice;
        }
        else
        {
            var hasRate = order.ExchangeRate.HasValue;
            var hasRateDate = order.ExchangeRateDate.HasValue;

            if (hasRate != hasRateDate)
            {
                return Error.Failure(
                    "Currency.InvalidExchangeRateSnapshot",
                    "The order exchange rate snapshot is incomplete.");
            }

            if (hasRate && hasRateDate)
            {
                var conversionResult =
                    await conversionService.ConvertAtRateAsync(
                        originUnitPrice,
                        currency,
                        order.ExchangeRate!.Value,
                        cancellationToken);

                if (conversionResult.IsFailure)
                    return conversionResult.Error;

                saleUnitPrice = conversionResult.Value;
            }
            else
            {
                var conversionResult =
                    await conversionService.ConvertWithRateAsync(
                        originUnitPrice,
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

                saleUnitPrice = conversion.ConvertedAmount;
            }
        }

        var addItemResult = order.AddItem(
            product.Id,
            product.Name,
            product.Sku,
            originUnitPrice,
            saleUnitPrice,
            request.Quantity);

        if (addItemResult.IsFailure)
            return addItemResult.Error;

        await orderRepository.UpdateAsync(
            order,
            cancellationToken);

        return Result.Success();
    }
}
