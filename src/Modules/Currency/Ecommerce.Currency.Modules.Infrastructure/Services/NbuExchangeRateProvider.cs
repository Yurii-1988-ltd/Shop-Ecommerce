using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Ecommerce.Currency.Modules.Application.Abstractions;
using Ecommerce.Currency.Modules.Application.Models;
using Ecommerce.Currency.Modules.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Ecommerce.Currency.Modules.Infrastructure.Services;

internal sealed class NbuExchangeRateProvider(
    HttpClient httpClient,
    IOptions<NbuOptions> options) : IExchangeRateProvider
{
    private readonly string _baseUrl = options.Value.BaseUrl.TrimEnd('/');

    public Task<IReadOnlyCollection<ExchangeRateData>> GetRatesAsync(
        CancellationToken cancellationToken = default)
    {
        return FetchRatesAsync(
            $"{_baseUrl}?json",
            cancellationToken);
    }

    public Task<IReadOnlyCollection<ExchangeRateData>> GetRatesAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var formattedDate = date.ToString(
            "yyyyMMdd",
            CultureInfo.InvariantCulture);

        return FetchRatesAsync(
            $"{_baseUrl}?date={formattedDate}&json",
            cancellationToken);
    }

    private async Task<IReadOnlyCollection<ExchangeRateData>> FetchRatesAsync(
        string url,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetFromJsonAsync<NbuExchangeRateDto[]>(
            url,
            cancellationToken);

        if (response is null || response.Length==0)
        {
            throw new HttpRequestException(
                "The NBU API returned an empty response.");
        }

        var rates = new List<ExchangeRateData>(response.Length);

        foreach (var item in response)
        {
            if (string.IsNullOrWhiteSpace(item.CurrencyCode))
            {
                throw new FormatException(
                    "The NBU API returned an empty currency code.");
            }

            if (item.Rate <= 0)
            {
                throw new FormatException(
                    $"The NBU API returned an invalid rate for {item.CurrencyCode}.");
            }

            if (!DateOnly.TryParseExact(
                    item.ExchangeDate,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var rateDate))
            {
                throw new FormatException(
                    $"Invalid exchange date returned by NBU: '{item.ExchangeDate}'.");
            }

            rates.Add(new ExchangeRateData(
                item.CurrencyCode.Trim().ToUpperInvariant(),
                item.Rate,
                Units: 1,
                rateDate));
        }

        return rates;
    }

    private sealed class NbuExchangeRateDto
    {
        [JsonPropertyName("cc")]
        public string CurrencyCode { get; init; } = string.Empty;

        [JsonPropertyName("rate")]
        public decimal Rate { get; init; }

        [JsonPropertyName("exchangedate")]
        public string ExchangeDate { get; init; } = string.Empty;
    }
}