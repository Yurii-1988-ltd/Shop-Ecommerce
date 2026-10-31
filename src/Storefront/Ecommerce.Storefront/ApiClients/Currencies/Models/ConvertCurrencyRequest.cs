namespace Ecommerce.Storefront.ApiClients.Currencies.Models;

public sealed record ConvertCurrencyRequest(
    decimal Amount,
    string SourceCurrency,
    string TargetCurrency);