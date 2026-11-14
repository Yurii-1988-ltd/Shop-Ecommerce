using Ecommerce.Shippings.Modules.Application.Features.GetShippingMethods;
using Ecommerce.Shippings.Modules.Application.Dto;
using MediatR;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class GetShippingMethodsEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/shippings/methods", async (
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
            .WithName("GetShippingMethods")
            .WithTags(Tags.Shippings)
            .Produces<IReadOnlyCollection<ShippingMethodResponse>>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}