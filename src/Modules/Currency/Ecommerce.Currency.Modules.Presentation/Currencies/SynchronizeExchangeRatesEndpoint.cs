using Ecommerce.Currency.Modules.Application.Features.SynchronizeExchangeRates;


namespace Ecommerce.Currency.Modules.Presentation.Currencies;

internal sealed class SynchronizeExchangeRatesEndpoint
{
    public void MapEndPoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/currencies/synchronize",
            async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new SynchronizeExchangeRatesCommand(),
                    cancellationToken);
                return result.IsSuccess
    ? Results.Ok(new { SynchronizedCount = result.Value })
    : Results.Problem(
        title: "Currency synchronization failed",
        detail: result.Error.ToString(),
        statusCode: StatusCodes.Status400BadRequest);
            })
            .WithTags(Tags.Currency)
            .WithName("SynchronizeExchangeRates")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}