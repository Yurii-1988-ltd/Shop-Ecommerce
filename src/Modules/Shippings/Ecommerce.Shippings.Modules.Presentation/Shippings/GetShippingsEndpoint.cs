using Ecommerce.Application.Pagination;
using Ecommerce.Shipings.Modules.Domain.Enums;
using Ecommerce.Shippings.Modules.Application.Features.GetShippings;


namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class GetShippingsEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/shippings", async (
                int page,
                int pageSize,
                ShippingStatus? status,
                string? search,
                ISender sender,
                CancellationToken cancellationToken) =>
        {
            var query = new GetShippingsQuery(
                page,
                pageSize,
                status,
                search);

            var result = await sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.Ok(result.Value);
        })
            .WithName("GetShippings")
            .WithTags(Tags.Shippings)
            .Produces<PagedResult<ShippingResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}