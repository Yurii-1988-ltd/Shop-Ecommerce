using Ecommerce.Order.Modules.Application.Features.GetBySlug;
using Ecommerce.Order.Modules.Application.Features.Responses;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ecommerce.Order.Modules.Presentation.Orders;

internal sealed class GetOrderBySlugEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/by-slug/{slug}", async (
            string slug,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(slug))
                return Results.BadRequest("Slug is required.");

            var result = await sender.Send(new GetBySlugQuery(slug), cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Error);
        })
        .WithTags(Tags.Orders)
        .WithName("GetOrderBySlug")
        .Produces<OrderResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }
}