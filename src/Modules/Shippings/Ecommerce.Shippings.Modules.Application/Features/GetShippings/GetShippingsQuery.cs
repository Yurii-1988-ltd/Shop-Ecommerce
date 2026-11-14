using Ecommerce.Application.Pagination;
using Ecommerce.Shipings.Modules.Domain.Enums;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippings;

public sealed record GetShippingsQuery(
    int Page = 1,
    int PageSize = 20,
    ShippingStatus? Status = null,
    string? Search = null)
    : PagedQuery<ShippingResponse>(Page, PageSize);