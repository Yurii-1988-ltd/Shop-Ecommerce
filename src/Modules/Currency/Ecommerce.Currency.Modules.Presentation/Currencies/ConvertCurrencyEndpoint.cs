
using Ecommerce.Currency.Modules.Application.Features.ConvertCurrency;


namespace Ecommerce.Currency.Modules.Presentation.Currencies;

internal sealed class ConvertCurrencyEndpoint
{
    public void MapEndPoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/currencies/convert",
            async (
                ConvertCurrencyRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new ConvertCurrencyCommand(
                    request.Amount,
                    request.SourceCurrency,
                    request.TargetCurrency);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                return result.IsSuccess
                    ? Results.Ok(new ConvertCurrencyResponse(
                        result.Value.Amount,
                        result.Value.Currency))
                    : Results.BadRequest(result.Error);
            })
            .WithTags(Tags.Currency)
            .WithName("ConvertCurrency")
            .Produces<ConvertCurrencyResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}

internal sealed record ConvertCurrencyRequest(
    decimal Amount,
    string SourceCurrency,
    string TargetCurrency);

internal sealed record ConvertCurrencyResponse(
    decimal Amount,
    string Currency);
