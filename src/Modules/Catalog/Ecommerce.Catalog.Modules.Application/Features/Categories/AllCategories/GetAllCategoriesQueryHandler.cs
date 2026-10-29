

using Ecommerce.Catalog.Modules.Application.Features.Categories.GetCategories;
using Ecommerce.Catalog.Modules.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Catalog.Modules.Application.Features.Categories.AllCategories;

internal sealed class GetAllCategoriesQueryHandler(ICatalogDatabase context) : IQueryHandler<GetAllCategoriesQuery, List<CategoryListItemResponse>>
{
    public async Task<Result<List<CategoryListItemResponse>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Category>.Filter.Eq(x => x.IsActive, true);

        var categories = await context.Categories
            .Find(filter)
            .SortBy(x => x.Name) 
            .ToListAsync(cancellationToken);

        var items = categories
            .Select(x => x.ToCategoryListItemResponse())
            .ToList();

        return Result<List<CategoryListItemResponse>>.Success(items);
    }
}
