


using Ecommerce.Shippings.Modules.Application.Features.MarkInTransit;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class MarkInTransitEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/shippings/{id:guid}/in-transit", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new MarkInTransitCommand(id),
                cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.NoContent();
        })
            .WithName("MarkShippingAsInTransit")
            .WithTags(Tags.Shippings)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
