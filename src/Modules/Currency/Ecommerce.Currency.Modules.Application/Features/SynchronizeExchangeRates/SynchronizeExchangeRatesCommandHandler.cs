
using Ecommerce.Application.CQRS;
using Ecommerce.Currency.Modules.Application.Abstractions;
using Ecommerce.Currency.Modules.Domain.Entities;
using Ecommerce.Domain.Domain;

namespace Ecommerce.Currency.Modules.Application.Features.SynchronizeExchangeRates;

internal sealed class SynchronizeExchangeRatesCommandHandler(IExchangeRateRepository repository,
                IExchangeRateProvider rateProvider) : ICommandHandler<SynchronizeExchangeRatesCommand, int>
{
    public async Task<Result<int>> Handle(SynchronizeExchangeRatesCommand request, CancellationToken cancellationToken)
    {
        //fetch the latest exchange rates from =NNU
        var rateData = await rateProvider.GetRatesAsync(cancellationToken);
        var exchangeRates = new List<ExchangeRate>(rateData.Count);
        foreach(var data in rateData)
        {
            var result = ExchangeRate.Create(
                data.CurrencyCode,
                data.Rate,
                data.Units,
                data.RateDate);
            if(result.IsFailure)
            {
                return Result.Failure<int>(result.Error);
            }
            exchangeRates.Add(result.Value);
        }
        await repository.UpsertRangeAsync(
    exchangeRates,
    cancellationToken);


        return Result.Success(exchangeRates.Count);
    }
}
