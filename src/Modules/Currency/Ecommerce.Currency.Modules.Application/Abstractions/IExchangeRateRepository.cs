

using Ecommerce.Currency.Modules.Domain.Entities;

namespace Ecommerce.Currency.Modules.Application.Abstractions;

public interface IExchangeRateRepository
{
    Task<ExchangeRate?> GetLatestAsync(
        string currencyCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ExchangeRate>> GetLatestRatesAsync(
        CancellationToken cancellationToken = default);

    Task UpsertAsync(
        ExchangeRate exchangeRate,
        CancellationToken cancellationToken = default);
}
