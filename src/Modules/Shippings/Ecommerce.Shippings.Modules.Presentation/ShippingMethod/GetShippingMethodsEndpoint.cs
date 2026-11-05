using Ecommerce.Shippings.Modules.Application.Features.GetShippingMethods;

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

internal sealed class GetShippingMethodsEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/shipping", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetShippingMethodsQuery(),
                    cancellationToken);

                if (result.IsFailure)
                    return Results.BadRequest(result.Error);

                return Results.Ok(result.Value);
            })
            .WithTags(Tags.Shipping);
    }
}