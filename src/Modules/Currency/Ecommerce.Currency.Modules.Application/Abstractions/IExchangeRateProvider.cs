using Ecommerce.Currency.Modules.Application.Models;

namespace Ecommerce.Currency.Modules.Application.Abstractions;

public interface IExchangeRateProvider
{
    Task<IReadOnlyCollection<ExchangeRateData>> GetRatesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ExchangeRateData>> GetRatesAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);
}