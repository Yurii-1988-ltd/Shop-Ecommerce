
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Currency.Modules.Infrastructure.Services;

internal sealed class CurrencyConversionService(
    IExchangeRateRepository exchangeRateRepository,
    IMemoryCache cache)
    : ICurrencyConversionService
{
    private static readonly TimeSpan CacheDuration =
    TimeSpan.FromHours(1);
    public async Task<Result<Money>> ConvertAsync(
        Money source,
        string targetCurrency,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(targetCurrency))
        {
            return Result.Failure<Money>(
                Error.Validation(
                    "Currency.TargetRequired",
                    "Target currency is required."));
        }

        targetCurrency = targetCurrency.Trim().ToUpperInvariant();

        if (targetCurrency.Length != 3 ||
            !targetCurrency.All(char.IsAsciiLetter))
        {
            return Result.Failure<Money>(
                Error.Validation(
                    "Currency.InvalidTarget",
                    "Target currency code must contain three letters."));
        }

        var sourceCurrency = source.Currency.Trim().ToUpperInvariant();

        if (sourceCurrency == targetCurrency)
        {
            return Money.Create(source.Amount, targetCurrency);
        }

        var sourceRate = await GetRatePerUnitAsync(
            sourceCurrency,
            cancellationToken);

        if (sourceRate.IsFailure)
        {
            return Result.Failure<Money>(sourceRate.Error);
        }

        var targetRate = await GetRatePerUnitAsync(
            targetCurrency,
            cancellationToken);

        if (targetRate.IsFailure)
        {
            return Result.Failure<Money>(targetRate.Error);
        }

        var convertedAmount = source.Amount
    * sourceRate.Value
    / targetRate.Value;

        convertedAmount = Math.Round(
            convertedAmount,
            2,
            MidpointRounding.AwayFromZero);

        return Money.Create(convertedAmount, targetCurrency);
    }

    private async Task<Result<decimal>> GetRatePerUnitAsync(
        string currencyCode,
        CancellationToken cancellationToken)
    {
        if (currencyCode == "UAH")
        
            return Result.Success(1m);
        var cacheKey = $"exchange-rate:{currencyCode}";
        if(cache.TryGetValue(cacheKey, out decimal cachedRate))
        {
            return Result.Success(cachedRate);
        }
        

        var exchangeRate = await exchangeRateRepository.GetLatestAsync(
            currencyCode,
            cancellationToken);

        if (exchangeRate is null)
        {
            return Result.Failure<decimal>(
                Error.NotFound(
                    "Currency.ExchangeRateNotFound",
                    $"Exchange rate for '{currencyCode}' was not found."));
        }
        var ratePerUnit = exchangeRate.GetRatePerUnit();
        cache.Set(cacheKey,
            ratePerUnit,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });

        return Result.Success(ratePerUnit);
    }
}