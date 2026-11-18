
using Ecommerce.Shippings.Modules.Application.Features.MarkShippingAsShipped;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class MarkShippingAsShippedEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/shippings/{id:guid}/mark-as-shipped", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new MarkShippingAsShippedCommand(id);
            var result = await sender.Send(command, cancellationToken);
            if (result.IsFailure)
                return Results.BadRequest(result.Error);
            return Results.NoContent();
        })
        .WithName("MarkShippingAsShipped")
        .WithTags(Tags.Shippings)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
