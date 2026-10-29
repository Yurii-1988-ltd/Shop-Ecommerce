using Ecommerce.Catalog.Modules.Application.Features.Categories.AllCategories;

using Ecommerce.Catalog.Modules.Application.Features.Categories.GetCategories;

namespace Ecommerce.Catalog.Modules.Presentation.Categories;

internal sealed class GetAllCategoriesEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/categories/all", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllCategoriesQuery());

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.Ok(result.Value);
        })
        .WithTags(Tags.Categories)
        .Produces<List<CategoryListItemResponse>>(StatusCodes.Status200OK);
    }
}