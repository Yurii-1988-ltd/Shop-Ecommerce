using Ecommerce.Application.CQRS;
using Ecommerce.Domain.ValueObjects;

namespace Ecommerce.Currency.Modules.Application.Features.ConvertCurrency;

public sealed record ConvertCurrencyCommand(decimal Amount,
string SourceCurrency,
string TargetCurrency) : ICommand<Money>;

