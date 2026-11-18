




using Ecommerce.Shippings.Modules.Application.Features.UpdateRecipientDetails;

internal sealed class UpdateRecipientDetailsEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/shippings/{id:guid}/recipient", async (
                Guid id,
                UpdateRecipientDetailsRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var command = new UpdateRecipientDetailsCommand(
                id,
                request.RecipientName,
                request.RecipientPhone,
                request.Country,
                request.City,
                request.Street,
                request.Building,
                request.PostalCode,
                request.DeliveryPoint);

            var result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.NoContent();
        })
            .WithName("UpdateRecipientDetails")
            .WithTags(Tags.Shippings)
            .Accepts<UpdateRecipientDetailsRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

public sealed record UpdateRecipientDetailsRequest(
    string RecipientName,
    string RecipientPhone,
    string Country,
    string City,
    string? Street,
    string? Building,
    string? PostalCode,
    string? DeliveryPoint);
public sealed record UpdateShippingRecipientRequest(
    string RecipientName,
    string RecipientPhone,
    string Country,
    string City,
    string? Street,
    string? Building,
    string? PostalCode,
    string? DeliveryPoint
);
