

using Ecommerce.Shippings.Modules.Application.Features.StartProcessing;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class StartProcessingEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/shippings/{id:guid}/start-processing", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new StartProcessingCommand(id),
                cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.NoContent();
        })
            .WithName("StartShippingProcessing")
            .WithTags(Tags.Shippings)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}