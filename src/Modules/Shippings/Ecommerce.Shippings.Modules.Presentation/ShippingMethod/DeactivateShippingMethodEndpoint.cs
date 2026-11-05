using Ecommerce.Shippings.Modules.Application.Features.DeactivateShippingMethod;

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

internal sealed class DeactivateShippingMethodEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPatch("/shipping/{id:guid}/deactivate", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new DeactivateShippingMethodCommand(id),
                    cancellationToken);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            })
            .WithTags(Tags.Shipping)
            .Produces<ShippingMethodResponse>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }
}