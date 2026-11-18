

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class GetShippingMethodByIdEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/shippings/methods/{id:guid}", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetShippingMethodByIdQuery(id),
                cancellationToken);

            if (result.IsFailure)
                return Results.NotFound(result.Error);

            return Results.Ok(result.Value);
        })
            .WithName("GetShippingMethodById")
            .WithTags(Tags.Shippings)
            .Produces<ShippingMethodResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}