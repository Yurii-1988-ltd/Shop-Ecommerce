using Ecommerce.Shippings.Modules.Application.Features.DeleteShippingMethod;
using MediatR;

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

internal sealed class DeleteShippingMethodEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/shippings/methods/{id:guid}", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new DeleteShippingMethodCommand(id),
                cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.NoContent();
        })
            .WithName("DeleteShippingMethod")
            .WithTags(Tags.Shippings)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}