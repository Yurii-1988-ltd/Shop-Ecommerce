using Ecommerce.Shippings.Modules.Application.Features.UpdateShippingMethod;

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;


using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;



public sealed class UpdateShippingMethodEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/shipping/{shippingId:guid}", async (
                Guid shippingId,
                UpdateShippingMethodRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateShippingMethodCommand(
                    shippingId,
                    request.Name,
                    request.Code,
                    request.Description,
                    request.Price,
                    request.Currency);

                var result = await sender.Send(command, cancellationToken);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Error);
            })
            .WithTags(Tags.Shipping)
            .WithName("UpdateShippingMethod")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
public sealed record UpdateShippingMethodRequest(
    string Name,
    string Code,
    string? Description,
    decimal Price,
    string Currency);