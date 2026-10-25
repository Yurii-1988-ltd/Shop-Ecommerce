namespace Ecommerce.Currency.Modules.Presentation.Currencies;

internal sealed class GetLatestExchangeRatesEndpoint
{
    public void MapEndPoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/currencies",
            async (
                [AsParameters]GetLatestExchangeRatesQuery query,
                ISender sender, 
                CancellationToken cancellationToken = default) =>
            {
                var result = await sender.Send(query, cancellationToken);
                return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);          

            })
            .WithTags(Tags.Currency)
            .WithName("GetLatestExchangeRates")
            .Produces<ReadOnlyCollection<ExchangeRateResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}
