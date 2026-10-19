
using Ecommerce.Application.Pagination;
using Ecommerce.Modules.Users.Contracts.Abstractions;
using Ecommerce.Modules.Users.Contracts.Dto;
using Ecommerce.Shared.Contracts.Orders;

namespace Ecommerce.Order.Modules.Application.Features.GetOrders;

internal sealed class GetOrdersQueryHandler(
    IOrderRepository orderRepository,
    IUserQueries userQueries)
    : IQueryHandler<GetOrdersQuery, PagedResult<OrderListResponse>>
{
    public async Task<Result<PagedResult<OrderListResponse>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await orderRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.Status,
            request.CustomerId,
            request.From,
            request.To,
            request.SortBy,
            request.Descending,
            cancellationToken);

        if (orders.Count == 0)
        {
            return Result.Success(new PagedResult<OrderListResponse>
            {
                Items = [],
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
        }

        var customerIds = orders
            .Where(x => x.CustomerId.HasValue)
            .Select(x => x.CustomerId!.Value)
            .Distinct()
            .ToList();

        var usersDictionary = new Dictionary<Guid, UserDto>();

        foreach (var customerId in customerIds)
        {
            var user = await userQueries.GetByIdAsync(
                customerId,
                cancellationToken);

            if (user is not null)
            {
                usersDictionary[user.Id] = user;
            }
        }

        var items = orders.Select(order =>
        {
            UserDto? user = null;

            if (order.CustomerId.HasValue)
            {
                usersDictionary.TryGetValue(
                    order.CustomerId.Value,
                    out user);
            }

            return order.ToListResponse(user);
        }).ToList();

        return Result.Success(new PagedResult<OrderListResponse>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}
