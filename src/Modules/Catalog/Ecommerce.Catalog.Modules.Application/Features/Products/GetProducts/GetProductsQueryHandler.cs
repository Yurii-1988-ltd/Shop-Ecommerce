

namespace Ecommerce.Catalog.Modules.Application.Features.Products.GetProducts;

internal sealed class GetProductsQueryHandler(
    ICatalogDatabase context,
    CategoryTreeService categoryTreeService) 
    : IQueryHandler<GetProductsQuery, PagedResult<ProductListItemResponse>>
{
    public async Task<Result<PagedResult<ProductListItemResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        // 0. Если передан CategoryId, собираем саму категорию и все ее подкатегории
        List<Guid>? targetCategoryIds = null;
        if (request.CategoryId.HasValue)
        {
            targetCategoryIds = await categoryTreeService.GetCategoryAndSubCategoryIdsAsync(
                request.CategoryId.Value,
                cancellationToken);
        }

        
        var spec = new ProductFilterAndSortSpec(
            categoryIds: targetCategoryIds, 
            brandId: request.BrandId,
            minPrice: request.MinPrice,
            maxPrice: request.MaxPrice,
            inStockOnly: request.InStockOnly,
            sortOption: request.SortOption,
            page: request.Page,
            pageSize: request.PageSize);

        // 2. Получаем IQueryable от MongoDB
        var query = context.Products.AsQueryable();

        // Поиск по строке
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(search) ||
                x.Sku.ToLower().Contains(search) ||
                x.ProductNumber.ToLower().Contains(search));
        }

        // 3. Подсчет общего количества ДО применения Skip/Take
        var countSpec = new ProductFilterAndSortSpec(
            categoryIds: targetCategoryIds, 
            brandId: request.BrandId,
            minPrice: request.MinPrice,
            maxPrice: request.MaxPrice,
            inStockOnly: request.InStockOnly,
            sortOption: request.SortOption,
            page: 0,
            pageSize: 0);

        var totalCountQuery = SpecificationEvaluator.Default.GetQuery(query, countSpec);
        var totalCount = await totalCountQuery.CountAsync(cancellationToken);

        // 4. Применяем полную спецификацию
        var filteredQuery = SpecificationEvaluator.Default.GetQuery(query, spec);
        var products = await filteredQuery.ToListAsync(cancellationToken);

        // 5. Маппинг и возврат результата
        var items = products
            .Select(x => x.ToListItemResponse())
            .ToList();

        return Result<PagedResult<ProductListItemResponse>>.Success(
            new PagedResult<ProductListItemResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }
}