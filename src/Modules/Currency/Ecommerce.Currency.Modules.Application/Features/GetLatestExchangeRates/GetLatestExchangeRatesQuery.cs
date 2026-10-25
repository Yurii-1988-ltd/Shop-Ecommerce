

using Ecommerce.Application.CQRS;
using Ecommerce.Currency.Modules.Application.Models;

namespace Ecommerce.Currency.Modules.Application.Features.GetLatestExchangeRates;

public sealed record GetLatestExchangeRatesQuery
    : IQuery<IReadOnlyCollection<ExchangeRateResponse>>;

