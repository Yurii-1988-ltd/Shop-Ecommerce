namespace Ecommerce.Storefront.ApiClients.Currencies.Models;

public sealed record ExchangeRateResponse(
    string CurrencyCode,
    decimal Rate,
    int Units,
    decimal RatePerUnit,
    DateOnly RateDate);