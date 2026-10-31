using Ecommerce.Storefront.ApiClients.Currencies.Models;
using Ecommerce.Storefront.ApiClients.Currencies.Services;

namespace Ecommerce.Storefront.Services;

public sealed class CurrencyState(ICurrencyApiClient currencyApiClient)
{
    public const string CookieName = "Ecommerce.Currency";

    private static readonly HashSet<string> SupportedCurrencies =
    [
        "UAH", "USD", "EUR"
    ];

    private IReadOnlyCollection<ExchangeRateResponse> _rates = [];
    private Task? _ratesLoadingTask;

    public string SelectedCurrency { get; private set; } = "UAH";

    public event Action? Changed;

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        _ratesLoadingTask ??= LoadRatesAsync(cancellationToken);

        await _ratesLoadingTask;
    }

    private async Task LoadRatesAsync(CancellationToken cancellationToken)
    {
        _rates = await currencyApiClient.GetLatestAsync(cancellationToken);
    }

    public void SetCurrency(string currency)
    {
        if (!SupportedCurrencies.Contains(currency))
            return;

        if (SelectedCurrency == currency)
            return;

        SelectedCurrency = currency;
        Changed?.Invoke();
    }

    public decimal Convert(
        decimal amount,
        string sourceCurrency)
    {
        if (sourceCurrency == SelectedCurrency)
            return amount;

        var amountInUah = sourceCurrency == "UAH"
            ? amount
            : amount * GetRate(sourceCurrency);

        return SelectedCurrency == "UAH"
            ? amountInUah
            : amountInUah / GetRate(SelectedCurrency);
    }

    private decimal GetRate(string currency)
    {
        var rate = _rates.FirstOrDefault(
            x => x.CurrencyCode == currency);

        if (rate is null || rate.RatePerUnit <= 0)
        {
            throw new InvalidOperationException(
                $"Exchange rate for {currency} is unavailable.");
        }

        return rate.RatePerUnit;
    }

    public string FormatPrice(decimal amount, string sourceCurrency)
    {
        var converted = Convert(amount, sourceCurrency);

        var symbol = SelectedCurrency switch
        {
            "UAH" => "₴",
            "USD" => "$",
            "EUR" => "€",
            _ => SelectedCurrency
        };

        return $"{converted:N2} {symbol}";
    }
}