using Ecommerce.Shippings.Modules.Application.Features.GetShippingById;

public sealed class GetShippingByIdEndpoint
{
    public  void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/shippings/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetShippingByIdQuery(id),
                cancellationToken);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetShippingById")
        .WithTags(Tags.Shippings)
        .Produces<ShippingResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}