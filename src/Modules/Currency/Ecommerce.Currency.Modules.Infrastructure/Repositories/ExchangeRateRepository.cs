
using Ecommerce.Currency.Modules.Domain.Entities;

namespace Ecommerce.Currency.Modules.Infrastructure.Repositories;

internal sealed class ExchangeRateRepository(ExchangeRateContext context) : IExchangeRateRepository
{
    public async Task<ExchangeRate?> GetLatestAsync(string currencyCode, CancellationToken cancellationToken = default)
    {
        currencyCode = NormalizeCurrencyCode(currencyCode);
        return await context.ExchangeRates
            .AsNoTracking()
            .Where(x => x.CurrencyCode == currencyCode)
            .OrderByDescending(x => x.RateDate) // Исправлено: получаем самую свежую дату
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ExchangeRate>> GetLatestRatesAsync(CancellationToken cancellationToken = default)
    {
     
        var latestDates = context.ExchangeRates
            .AsNoTracking()
            .GroupBy(x => x.CurrencyCode)
            .Select(group => new
            {
                CurrencyCode = group.Key,
                RateDate = group.Max(x => x.RateDate)
            });

       
        return await (from rate in context.ExchangeRates.AsNoTracking()
                      join latest in latestDates
                      on new { rate.CurrencyCode, rate.RateDate }
                      equals new { latest.CurrencyCode, latest.RateDate }
                      select rate)
                     .ToListAsync(cancellationToken);
    }

    public async Task UpsertAsync(ExchangeRate exchangeRate, CancellationToken cancellationToken = default)
    {
        var existing = await context.ExchangeRates
            .FirstOrDefaultAsync(x => x.CurrencyCode == exchangeRate.CurrencyCode && x.RateDate == exchangeRate.RateDate, cancellationToken);

        if (existing is null)
        {
            await context.ExchangeRates.AddAsync(exchangeRate, cancellationToken);
        }
        else
        {
            var result = existing.Update(exchangeRate.Rate, exchangeRate.Units, exchangeRate.RateDate);
            if (result.IsFailure)
            {
                throw new InvalidOperationException($"Failed to update exchange rate: {result.Error}");
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizeCurrencyCode(string currencyCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        return currencyCode.Trim().ToUpperInvariant();
    }

    public async Task UpsertRangeAsync(
     IReadOnlyCollection<ExchangeRate> exchangeRates,
     CancellationToken cancellationToken = default)
    {
        // Return early if there are no exchange rates to process.
        if (exchangeRates.Count == 0)
            return;

        // Collect distinct currency codes from the incoming exchange rates.
        var currencyCodes = exchangeRates
            .Select(x => x.CurrencyCode)
            .Distinct()
            .ToArray();

        // Collect distinct rate dates from the incoming exchange rates.
        var dates = exchangeRates
            .Select(x => x.RateDate)
            .Distinct()
            .ToArray();

        // Load existing records matching the incoming currency codes and dates.
        // These records will be updated instead of creating duplicates.
        var existingRates = await context.ExchangeRates
            .Where(x => currencyCodes.Contains(x.CurrencyCode)
                        && dates.Contains(x.RateDate))
            .ToListAsync(cancellationToken);

        // Create a lookup using currency code and rate date as the composite key.
        var existingLookup = existingRates.ToDictionary(
            x => (x.CurrencyCode, x.RateDate));

        // Process each incoming exchange rate.
        foreach (var rate in exchangeRates)
        {
            // If a record with the same currency code and date exists,
            // update its rate and units.
            if (existingLookup.TryGetValue(
                    (rate.CurrencyCode, rate.RateDate),
                    out var existing))
            {
                var result = existing.Update(
                    rate.Rate,
                    rate.Units,
                    rate.RateDate);

                // Throw an exception if the domain entity rejects the update.
                if (result.IsFailure)
                {
                    throw new InvalidOperationException(
                        $"Failed to update exchange rate for {rate.CurrencyCode}.");
                }
            }
            else
            {
                // Add a new record when no matching record exists.
                context.ExchangeRates.Add(rate);

                // Update the lookup to prevent duplicate additions
                // within the same batch.
                existingLookup.Add(
                    (rate.CurrencyCode, rate.RateDate),
                    rate);
            }
        }

        // Persist all additions and updates in a single database operation.
        await context.SaveChangesAsync(cancellationToken);
    }
}