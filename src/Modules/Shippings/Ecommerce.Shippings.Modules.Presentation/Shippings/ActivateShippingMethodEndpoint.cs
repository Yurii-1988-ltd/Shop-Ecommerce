



using Ecommerce.Shippings.Modules.Application.Features.ActivateShippingMethod;



internal sealed class ActivateShippingMethodEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPatch("/shippings/methods/{id:guid}/activate", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new ActivateShippingMethodCommand(id),
                cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.Ok(result.Value);
        })
            .WithName("ActivateShippingMethod")
            .WithTags(Tags.Shippings)
            .Produces<Guid>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
