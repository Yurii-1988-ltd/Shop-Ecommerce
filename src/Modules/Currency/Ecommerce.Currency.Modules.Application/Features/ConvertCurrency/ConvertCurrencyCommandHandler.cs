

using Ecommerce.Application.CQRS;
using Ecommerce.Currency.Modules.Application.Abstractions;
using Ecommerce.Domain.Domain;
using Ecommerce.Domain.ValueObjects;


namespace Ecommerce.Currency.Modules.Application.Features.ConvertCurrency;

internal sealed class ConvertCurrencyCommandHandler(ICurrencyConversionService currencyConversionService) : ICommandHandler<ConvertCurrencyCommand, Money>
{
    public async Task<Result<Money>> Handle(ConvertCurrencyCommand request, CancellationToken cancellationToken)
    {
        var sourceResult = Money.Create(request.Amount,
            request.SourceCurrency);

        if(sourceResult.IsFailure)
        {
            return Result.Failure<Money>(sourceResult.Error);
        }
        return await currencyConversionService.ConvertAsync(
            sourceResult.Value,
            request.TargetCurrency,
            cancellationToken);
     
    }
}
