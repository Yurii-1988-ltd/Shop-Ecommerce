using Ecommerce.Application.CQRS;
using Ecommerce.Currency.Modules.Application.Abstractions;
using Ecommerce.Currency.Modules.Application.Models;
using Ecommerce.Domain.Domain;

namespace Ecommerce.Currency.Modules.Application.Features.GetLatestExchangeRates;

internal sealed class GetLatestExchangeRatesQueryHandler(IExchangeRateRepository repository) : IQueryHandler<GetLatestExchangeRatesQuery, IReadOnlyCollection<ExchangeRateResponse>>
{
    public async Task<Result<IReadOnlyCollection<ExchangeRateResponse>>> Handle(GetLatestExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        var exchangeRates =
           await repository.GetLatestRatesAsync(cancellationToken);

        IReadOnlyCollection<ExchangeRateResponse> response =
            exchangeRates
                .Select(rate => new ExchangeRateResponse(
                    rate.CurrencyCode,
                    rate.Rate,
                    rate.Units,
                    rate.GetRatePerUnit(),
                    rate.RateDate))
                .ToArray();

        return Result.Success(response);
    }
}
