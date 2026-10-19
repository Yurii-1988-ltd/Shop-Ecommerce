

using Ecommerce.Domain.Domain;
using Ecommerce.Domain.ValueObjects;

namespace Ecommerce.Currency.Modules.Application.Abstractions;

public interface ICurrencyConversionService
{
    Task<Result<Money>> ConvertAsync(
        Money source,
        string targetCurrency,
        CancellationToken cancellationToken = default);
}
