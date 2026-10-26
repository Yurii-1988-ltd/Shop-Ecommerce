using Ecommerce.Domain.ValueObjects;

public sealed record CurrencyConversionResult(
    Money ConvertedAmount,
    decimal ExchangeRate,
    DateOnly? ExchangeRateDate,
    DateTime CapturedAtUtc);