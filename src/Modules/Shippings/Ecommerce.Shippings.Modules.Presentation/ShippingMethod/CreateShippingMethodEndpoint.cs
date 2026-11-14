using Ecommerce.Shippings.Modules.Application.Features.CreateShippingMethod;
using MediatR;

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

internal sealed class CreateShippingMethodEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/shippings/methods", async (
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

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.Created(
                $"/shippings/methods/{result.Value}",
                result.Value);
        })
            .WithTags(Tags.Shippings)
            .WithName("CreateShippingMethod")
            .Accepts<CreateShippingMethodRequest>("application/json")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
public sealed record CreateShippingMethodRequest(
    string Name,
    string Code,
    string? Description,
    decimal Price,
    string Currency);