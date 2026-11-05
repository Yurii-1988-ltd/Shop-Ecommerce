using Ecommerce.Shippings.Modules.Application.Features.CreateShipping;

internal sealed class CreateShippingEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
                CreateShippingRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateShippingCommand(
                    request.OrderId,
                    request.ShippingMethodId,
                    request.RecipientName,
                    request.RecipientPhone,
                    request.Country,
                    request.City,
                    request.Street,
                    request.Building,
                    request.PostalCode,
                    request.DeliveryPoint,
                    request.Notes);

                var result = await sender.Send(command, cancellationToken);

                if (result.IsFailure)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/shipping/{result.Value}",
                    result.Value);
            })
            .WithName("CreateShipping")
            .WithTags(Tags.Shipping)
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
public sealed record CreateShippingRequest(
    Guid OrderId,
    Guid ShippingMethodId,
    string RecipientName,
    string RecipientPhone,
    string Country,
    string City,
    string? Street,
    string? Building,
    string? PostalCode,
    string? DeliveryPoint,
    string? Notes);