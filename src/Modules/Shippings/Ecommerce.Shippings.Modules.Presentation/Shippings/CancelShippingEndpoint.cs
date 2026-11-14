using Ecommerce.Shippings.Modules.Application.Features.CancelShipping;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class CancelShippingEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/shippings/{id:guid}/cancel", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new CancelShippingCommand(id);

            var result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.NoContent();
        })
        .WithName("CancelShipping")
        .WithTags(Tags.Shippings)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}