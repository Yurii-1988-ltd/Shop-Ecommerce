using Ecommerce.Shippings.Modules.Application.Features.DeleteShippingMethod;

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

internal sealed class DeleteShippingMethodsEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/shipping/{id:guid}", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new DeleteShippingMethodCommand(id),
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