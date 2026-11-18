using Ecommerce.Shippings.Modules.Application.Features.DeactivateShippingMethod;

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

internal sealed class DeactivateShippingMethodEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPatch("/shippings/methods/{id:guid}/deactivate", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                     new DeactivateShippingMethodCommand(id),
                     cancellationToken);

                if (result.IsFailure)
                    return Results.BadRequest(result.Error);

                return Results.Ok(result.Value);
            })
               .WithName("DeactivateShippingMethod")
            .WithTags(Tags.Shippings)
            .Produces<Guid>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}