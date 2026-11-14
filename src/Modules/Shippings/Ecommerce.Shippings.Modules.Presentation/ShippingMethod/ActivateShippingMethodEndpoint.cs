using Ecommerce.Shippings.Modules.Application.Features.ActivateShippingMethod;

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

internal sealed class ActivateShippingMethodEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPatch("/shipping/{id:guid}/activate", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new ActivateShippingMethodCommand(id),
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