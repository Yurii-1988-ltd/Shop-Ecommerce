
using Ecommerce.Shippings.Modules.Application.Features.UpdateShippingMethod;
using MediatR;

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

internal sealed class UpdateShippingMethodEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/shippings/methods/{id:guid}", async (
                Guid id,
                UpdateShippingMethodRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var command = new UpdateShippingMethodCommand(
                id,
                request.Name,
                request.Code,
                request.Description,
                request.Price,
                request.Currency);

            var result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.Ok(result.Value);
        })
            .WithName("UpdateShippingMethod")
            .WithTags(Tags.Shippings)
            .Accepts<UpdateShippingMethodRequest>("application/json")
            .Produces<Guid>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
public sealed record UpdateShippingMethodRequest(
    string Name,
    string Code,
    string? Description,
    decimal Price,
    string Currency);