using Ecommerce.Cart.Modules.Application.RemoveCartById;

namespace Ecommerce.Cart.Modules.Presentation.Carts;

internal sealed class RemoveCartByIdEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/admin/carts/id/{cartId:guid}", async (
            Guid cartId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new RemoveCartByIdCommand(cartId),
                cancellationToken);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(result.Error);
        })
        .WithTags(Tags.Carts)
        .WithName("RemoveCartById");
    }
}