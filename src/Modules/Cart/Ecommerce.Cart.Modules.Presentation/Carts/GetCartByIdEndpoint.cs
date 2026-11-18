

using Ecommerce.Cart.Modules.Application.Features.GetCartById;

namespace Ecommerce.Cart.Modules.Presentation.Carts;

internal sealed class GetCartByIdEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/admin/carts/id/{cartId:guid}", async (
    Guid cartId,
    ISender sender,
    CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetCartByIdQuery(cartId),
                cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Error);
        });
    }
}
