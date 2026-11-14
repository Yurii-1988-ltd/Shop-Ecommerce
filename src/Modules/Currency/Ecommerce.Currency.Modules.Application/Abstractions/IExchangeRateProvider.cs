
using Ecommerce.Currency.Modules.Application.Models;

namespace Ecommerce.Currency.Modules.Application.Abstractions;

public interface IExchangeRateProvider
{
    Task<IReadOnlyCollection<ExchangeRateData>> GetRateAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ExchangeRateData>> GetRatesAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);
}
