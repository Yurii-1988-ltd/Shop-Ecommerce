using Ecommerce.Catalog.Modules.Domain.Entities;

namespace Ecommerce.Catalog.Modules.Application.Features.Categories.GetCategories;

internal sealed class GetCategoriesQueryHandler(ICatalogDatabase context) : IQueryHandler<GetCategoriesQuery, PagedResult<CategoryListItemResponse>>
{
    public async Task<Result<PagedResult<CategoryListItemResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Category>.Filter.Empty;

        var totalCount = (int)await context.Categories
            .CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var categories = await context.Categories
            .Find(filter)
            .Skip((request.Page - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = categories
            .Select(x => x.ToCategoryListItemResponse())
            .ToList();

        return Result<PagedResult<CategoryListItemResponse>>.Success(
     new PagedResult<CategoryListItemResponse>
     {
         Items = items,
         Page = request.Page,
         PageSize = request.PageSize,
         TotalCount = totalCount
     });
    }
}
