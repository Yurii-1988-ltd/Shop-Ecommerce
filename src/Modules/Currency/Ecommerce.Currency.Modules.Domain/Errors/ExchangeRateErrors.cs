
using Ecommerce.Domain.Domain;

namespace Ecommerce.Currency.Modules.Domain.Errors;

internal class ExchangeRateErrors
{
    public static Error CurrencyCodeIsRequired =>
      new(
          "ExchangeRate.CurrencyCodeIsRequired",
          "The currency code is required.",
          ErrorType.Validation);

    public static Error InvalidCurrencyCode =>
        new(
            "ExchangeRate.InvalidCurrencyCode",
            "The currency code must contain exactly 3 characters.",
            ErrorType.Validation);

    public static Error InvalidRate =>
        new(
            "ExchangeRate.InvalidRate",
            "The exchange rate must be greater than zero.",
            ErrorType.Validation);

    public static Error InvalidUnits =>
        new(
            "ExchangeRate.InvalidUnits",
            "The number of currency units must be greater than zero.",
            ErrorType.Validation);

    public static Error InvalidRateDate =>
        new(
            "ExchangeRate.InvalidRateDate",
            "The exchange rate date is invalid.",
            ErrorType.Validation);

    public static Error NotFound(string currencyCode) =>
        Error.NotFound(
            "ExchangeRate.NotFound",
            $"Exchange rate for currency '{currencyCode}' was not found.");

    public static Error NotFound(
        string currencyCode,
        DateOnly rateDate) =>
        Error.NotFound(
            "ExchangeRate.NotFound",
            $"Exchange rate for currency '{currencyCode}' on '{rateDate}' was not found.");
}

