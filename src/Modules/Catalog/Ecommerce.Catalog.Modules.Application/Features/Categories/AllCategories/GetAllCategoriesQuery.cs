

using Ecommerce.Catalog.Modules.Application.Features.Categories.GetCategories;

namespace Ecommerce.Catalog.Modules.Application.Features.Categories.AllCategories;

public sealed record GetAllCategoriesQuery() : IQuery<List<CategoryListItemResponse>>;

