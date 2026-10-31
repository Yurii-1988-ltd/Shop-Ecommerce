using Ecommerce.Storefront.ApiClients.Currencies.Models;

namespace Ecommerce.Storefront.ApiClients.Currencies.Services;

public interface ICurrencyApiClient
{
    Task<IReadOnlyCollection<ExchangeRateResponse>> GetLatestAsync(
        CancellationToken cancellationToken = default);

    Task<ConvertCurrencyResponse> ConvertAsync(
        ConvertCurrencyRequest request,
        CancellationToken cancellationToken = default);
}