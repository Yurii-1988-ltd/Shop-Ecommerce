using Ecommerce.Application.CQRS;
using Ecommerce.Application.Pagination;

namespace Ecommerce.Catalog.Modules.Application.Features.Brands.GetBrands;

public sealed record GetBrandsQuery(int Page, int PageSize)
    : PagedQuery<BrandListItemResponse>(Page, PageSize);