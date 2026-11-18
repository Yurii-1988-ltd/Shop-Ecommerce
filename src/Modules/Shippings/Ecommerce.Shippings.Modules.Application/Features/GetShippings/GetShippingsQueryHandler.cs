

using Ecommerce.Application.Pagination;
using Ecommerce.Shipings.Modules.Domain.Enums;

namespace Ecommerce.Shippings.Modules.Application.Features.GetShippings;

internal sealed class GetShippingsQueryHandler(
    IShippingRepository repository)
    : IQueryHandler<GetShippingsQuery, PagedResult<ShippingResponse>>
{
    public async Task<Result<PagedResult<ShippingResponse>>> Handle(
        GetShippingsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Status.HasValue &&
            !Enum.IsDefined(typeof(ShippingStatus), request.Status.Value))
        {
            return ShippingErrors.StatusIsRequired;
        }

        var pageRequest = new PageRequest
        {
            Page = Math.Max(1, request.Page),
            PageSize = Math.Clamp(request.PageSize, 1, 100)
        };

        var result = await repository.GetPagedAsync(
            pageRequest,
            request.Status,
            request.Search,
            cancellationToken);

        return new PagedResult<ShippingResponse>
        {
            Items = result.Items.Select(x => x.ToResponse()).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}