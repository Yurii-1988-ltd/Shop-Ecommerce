
namespace Ecommerce.Currency.Modules.Application.Models;

public sealed record ExchangeRateData(
    string CurrencyCode,
    decimal Rate,
    int Units,
    DateOnly RateDate);

