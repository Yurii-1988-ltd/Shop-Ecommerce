using System.Net.Http.Json;
using Ecommerce.Storefront.ApiClients.Currencies.Models;

namespace Ecommerce.Storefront.ApiClients.Currencies.Services;

public sealed class CurrencyApiClient(HttpClient httpClient)
    : ICurrencyApiClient
{
    public async Task<IReadOnlyCollection<ExchangeRateResponse>> GetLatestAsync(
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<
            IReadOnlyCollection<ExchangeRateResponse>>(
                "/currencies",
                cancellationToken) ?? [];
    }

    public async Task<ConvertCurrencyResponse> ConvertAsync(
        ConvertCurrencyRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "/currencies/convert",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ConvertCurrencyResponse>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException(
                   "Currency conversion response is empty.");
    }
}