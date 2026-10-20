

namespace Ecommerce.Currency.Modules.Application.Models;

public sealed record ExchangeRateResponse(
    string CurrencyCode,
    decimal Rate,
    int Units,
    decimal RatePerUnit,
    DateOnly RateDate);

