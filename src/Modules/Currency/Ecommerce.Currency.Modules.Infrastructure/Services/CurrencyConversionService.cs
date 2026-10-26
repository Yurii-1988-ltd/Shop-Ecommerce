
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
        var result = await ConvertWithRateAsync(
            source,
            targetCurrency,
            cancellationToken);

        if (result.IsFailure)
            return Result.Failure<Money>(result.Error);

        return Result.Success(result.Value.ConvertedAmount);
    }

    public async Task<Result<CurrencyConversionResult>> ConvertWithRateAsync(
        Money source,
        string targetCurrency,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(targetCurrency))
        {
            return Result.Failure<CurrencyConversionResult>(
                Error.Validation(
                    "Currency.TargetRequired",
                    "Target currency is required."));
        }

        targetCurrency = targetCurrency.Trim().ToUpperInvariant();

        if (targetCurrency.Length != 3 ||
            !targetCurrency.All(char.IsAsciiLetter))
        {
            return Result.Failure<CurrencyConversionResult>(
                Error.Validation(
                    "Currency.InvalidTarget",
                    "Target currency code must contain three letters."));
        }

        var sourceCurrency = source.Currency.Trim().ToUpperInvariant();

        var sourceRateResult = await GetRateAsync(
            sourceCurrency,
            cancellationToken);

        if (sourceRateResult.IsFailure)
        {
            return Result.Failure<CurrencyConversionResult>(
                sourceRateResult.Error);
        }

        var targetRateResult = await GetRateAsync(
            targetCurrency,
            cancellationToken);

        if (targetRateResult.IsFailure)
        {
            return Result.Failure<CurrencyConversionResult>(
                targetRateResult.Error);
        }

        var sourceRate = sourceRateResult.Value.RatePerUnit;
        var targetRate = targetRateResult.Value.RatePerUnit;

        decimal convertedAmount;

        if (sourceCurrency == targetCurrency)
        {
            convertedAmount = source.Amount;
        }
        else
        {
            convertedAmount = source.Amount * sourceRate / targetRate;

            convertedAmount = Math.Round(
                convertedAmount,
                2,
                MidpointRounding.AwayFromZero);
        }

        var moneyResult = Money.Create(
            convertedAmount,
            targetCurrency);

        if (moneyResult.IsFailure)
        {
            return Result.Failure<CurrencyConversionResult>(
                moneyResult.Error);
        }

        return Result.Success(
            new CurrencyConversionResult(
                moneyResult.Value,
                targetRate,
                targetRateResult.Value.RateDate,
                DateTime.UtcNow));
    }

    private async Task<Result<CachedExchangeRate>> GetRateAsync(
        string currencyCode,
        CancellationToken cancellationToken)
    {
        if (currencyCode == "UAH")
        {
            return Result.Success(
                new CachedExchangeRate(1m, null));
        }

        var cacheKey = $"exchange-rate:{currencyCode}";

        if (cache.TryGetValue(
                cacheKey,
                out CachedExchangeRate? cachedRate) &&
            cachedRate is not null)
        {
            return Result.Success(cachedRate);
        }

        var exchangeRate = await exchangeRateRepository.GetLatestAsync(
            currencyCode,
            cancellationToken);

        if (exchangeRate is null)
        {
            return Result.Failure<CachedExchangeRate>(
                Error.NotFound(
                    "Currency.ExchangeRateNotFound",
                    $"Exchange rate for '{currencyCode}' was not found."));
        }

        var rate = new CachedExchangeRate(
            exchangeRate.GetRatePerUnit(),
            exchangeRate.RateDate);

        cache.Set(
            cacheKey,
            rate,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });

        return Result.Success(rate);
    }
    public Task<Result<Money>> ConvertAtRateAsync(
     Money source,
     string targetCurrency,
     decimal exchangeRate,
     CancellationToken cancellationToken = default)
    {
        if (source is null)
        {
            return Task.FromResult(
                Result.Failure<Money>(
                    Error.Validation(
                        "Currency.SourceRequired",
                        "Source money is required.")));
        }

        if (string.IsNullOrWhiteSpace(targetCurrency))
        {
            return Task.FromResult(
                Result.Failure<Money>(
                    Error.Validation(
                        "Currency.TargetRequired",
                        "Target currency is required.")));
        }

        targetCurrency = targetCurrency.Trim().ToUpperInvariant();

        if (targetCurrency.Length != 3 ||
            !targetCurrency.All(char.IsAsciiLetter))
        {
            return Task.FromResult(
                Result.Failure<Money>(
                    Error.Validation(
                        "Currency.InvalidTarget",
                        "Target currency code must contain three letters.")));
        }

        var sourceCurrency = source.Currency.Trim().ToUpperInvariant();

        if (sourceCurrency != "UAH")
        {
            return Task.FromResult(
                Result.Failure<Money>(
                    Error.Validation(
                        "Currency.InvalidSource",
                        "Source currency must be UAH.")));
        }

        if (targetCurrency == "UAH")
        {
            return Task.FromResult(
                Money.Create(source.Amount, "UAH"));
        }

        if (exchangeRate <= 0)
        {
            return Task.FromResult(
                Result.Failure<Money>(
                    Error.Validation(
                        "Currency.InvalidExchangeRate",
                        "Exchange rate must be greater than zero.")));
        }

        var convertedAmount = Math.Round(
            source.Amount / exchangeRate,
            2,
            MidpointRounding.AwayFromZero);

        return Task.FromResult(
            Money.Create(convertedAmount, targetCurrency));
    }

    private sealed record CachedExchangeRate(
        decimal RatePerUnit,
        DateOnly? RateDate);
}
