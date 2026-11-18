
using Ecommerce.Domain.Domain;
using Ecommerce.Currency.Modules.Domain.Errors;

namespace Ecommerce.Currency.Modules.Domain.Entities;

#region private fields and constructors
public sealed class ExchangeRate : Entity
{
    public string CurrencyCode { get; private set; } = default!;
    public decimal Rate { get; private set; }
    public int Units { get; private set; }
    public DateOnly RateDate { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private ExchangeRate()
    {
    }
#endregion
    #region static factory methods
    public static Result<ExchangeRate> Create(
        string currencyCode,
        decimal rate,
        int units,
        DateOnly rateDate)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            return ExchangeRateErrors.CurrencyCodeIsRequired;
        }

        currencyCode = currencyCode.Trim().ToUpperInvariant();

        if (currencyCode.Length != 3 ||
            !currencyCode.All(char.IsAsciiLetter))
        {
            return ExchangeRateErrors.InvalidCurrencyCode;
        }

        if (rate <= 0)
        {
            return ExchangeRateErrors.InvalidRate;
        }

        if (units <= 0)
        {
            return ExchangeRateErrors.InvalidUnits;
        }

        var exchangeRate = new ExchangeRate
        {
            CurrencyCode = currencyCode,
            Rate = rate,
            Units = units,
            RateDate = rateDate,
            CreatedAtUtc = DateTime.UtcNow
        };

        return Result.Success(exchangeRate);
    }

    public decimal GetRatePerUnit()
    {
        return Rate / Units;
    }

    public Result Update(
        decimal rate,
        int units,
        DateOnly rateDate)
    {
        if (rate <= 0)
        {
            return ExchangeRateErrors.InvalidRate;
        }

        if (units <= 0)
        {
            return ExchangeRateErrors.InvalidUnits;
        }

        Rate = rate;
        Units = units;
        RateDate = rateDate;

        return Result.Success();
   
    }
    #endregion
}
