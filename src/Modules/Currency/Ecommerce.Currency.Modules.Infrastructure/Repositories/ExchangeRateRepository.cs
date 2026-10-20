using Ecommerce.Currency.Modules.Application.Abstractions;
using Ecommerce.Currency.Modules.Domain.Entities;
using Ecommerce.Currency.Modules.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
}