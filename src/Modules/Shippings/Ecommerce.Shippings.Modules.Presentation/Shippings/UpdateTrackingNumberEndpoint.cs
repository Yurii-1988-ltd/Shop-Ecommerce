



using Ecommerce.Shippings.Modules.Application.Features.UpdateTrackingNumber;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class UpdateTrackingNumberEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/shippings/{id:guid}/tracking-number", async (
            Guid id,
            UpdateTrackingNumberRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateTrackingNumberCommand(
                id,
                request.TrackingNumber);

            var result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.NoContent();
        })
        .WithName("UpdateTrackingNumber")
        .WithTags(Tags.Shippings)
            .Accepts<UpdateTrackingNumberRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
    
public sealed record UpdateTrackingNumberRequest(
    string TrackingNumber);