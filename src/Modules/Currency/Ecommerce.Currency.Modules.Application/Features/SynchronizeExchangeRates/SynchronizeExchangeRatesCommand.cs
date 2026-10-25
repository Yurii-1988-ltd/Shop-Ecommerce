

using Ecommerce.Application.CQRS;

namespace Ecommerce.Currency.Modules.Application.Features.SynchronizeExchangeRates;

public sealed record SynchronizeExchangeRatesCommand() : ICommand<int>;

