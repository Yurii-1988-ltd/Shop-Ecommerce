using Ecommerce.Shippings.Modules.Application.Features.CreateShippingMethod;


namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

public sealed class CreateShippingMethodEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/shipping", async (
                CreateShippingMethodRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateShippingMethodCommand(
                    request.Name,
                    request.Code,
                    request.Description,
                    request.Price,
                    request.Currency);

                var result = await sender.Send(command, cancellationToken);

                return result.IsSuccess
                    ? Results.Created($"/shipping/{result.Value}", result.Value)
                    : Results.BadRequest(result.Error);
            })
            .WithTags(Tags.Shipping)
            .WithName("CreateShippingMethod")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
public sealed record CreateShippingMethodRequest(
    string Name,
    string Code,
    string? Description,
    decimal Price,
    string Currency);