using Ecommerce.Shippings.Modules.Application.Features.MarkAsShipped;
using MediatR;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class MarkAsShippedEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/shippings/{id:guid}/ship", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new MarkAsShippedCommand(id),
                cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.NoContent();
        })
            .WithName("MarkShippingAsShipped")
            .WithTags(Tags.Shippings)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}