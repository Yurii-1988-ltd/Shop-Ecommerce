
using Ecommerce.Shippings.Modules.Application.Features.MarkAsReturned;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class MarkAsReturnedEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/shippings/{id:guid}/return", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new MarkAsReturnedCommand(id),
                cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.NoContent();
        })
            .WithName("MarkShippingAsReturned")
            .WithTags(Tags.Shippings)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
