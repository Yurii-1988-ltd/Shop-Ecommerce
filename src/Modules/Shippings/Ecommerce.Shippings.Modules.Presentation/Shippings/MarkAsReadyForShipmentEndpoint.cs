

using Ecommerce.Shippings.Modules.Application.Features.MarkAsReadyForShipment;


namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class MarkAsReadyForShipmentEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/shippings/{id:guid}/ready-for-shipment", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new MarkAsReadyForShipmentCommand(id),
                cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.NoContent();
        })
            .WithName(" MarkAsReadyForShipment")
            .WithTags(Tags.Shippings)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
