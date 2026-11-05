using Ecommerce.Shippings.Modules.Application.Features.GetShippings;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class GetShippingsEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/shippings", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetShippingsQuery(),
                cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.Ok(result.Value);
        })
        .WithName("GetShippings")
        .Produces<IReadOnlyList<ShippingResponse>>(
            StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}