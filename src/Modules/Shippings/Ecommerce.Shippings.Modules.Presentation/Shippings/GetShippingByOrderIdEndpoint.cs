using Ecommerce.Shippings.Modules.Application.Features.GetShippingByOrderId;
using MediatR;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class GetShippingByOrderIdEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/shipping/order/{orderId:guid}", async (
                Guid orderId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetShippingByOrderIdQuery(orderId),
                    cancellationToken);

                if (result.IsFailure)
                    return Results.NotFound(result.Error);

                return Results.Ok(result.Value);
            })
            .WithName("GetShippingByOrderId")
            .Produces<ShippingResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}