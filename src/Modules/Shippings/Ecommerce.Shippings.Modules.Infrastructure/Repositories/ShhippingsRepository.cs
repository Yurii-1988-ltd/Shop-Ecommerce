using Ecommerce.Application.Pagination;
using Ecommerce.Shipings.Modules.Domain.Entities;
using Ecommerce.Shipings.Modules.Domain.Enums;
using Ecommerce.Shippings.Modules.Domain.Entities;

namespace Ecommerce.Shippings.Modules.Infrastructure.Repositories;

public sealed class ShippingRepository(ShippingContext context)
    : IShippingRepository
{
    public async Task<Shipping?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Shippings
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Shipping?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await context.Shippings
            .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
    }

    public async Task<Shipping?> GetByTrackingNumberAsync(
        string trackingNumber,
        CancellationToken cancellationToken = default)
    {
        return await context.Shippings
            .FirstOrDefaultAsync(
                x => x.TrackingNumber == trackingNumber.Trim(),
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Shipping>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Shippings
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Shipping shipping,
        CancellationToken cancellationToken = default)
    {
        await context.Shippings.AddAsync(shipping, cancellationToken);
    }

    public void Update(Shipping shipping)
    {
        context.Shippings.Update(shipping);
    }

    public void Remove(Shipping shipping)
    {
        context.Shippings.Remove(shipping);
    }

    public async Task<PagedResult<Shipping>> GetPagedAsync(
        PageRequest request,
        ShippingStatus? status = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Shippings
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(x =>
                x.TrackingNumber != null &&
                EF.Functions.ILike(
                    x.TrackingNumber,
                    $"%{normalizedSearch}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenBy(x => x.Id)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Shipping>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.Take,
            TotalCount = totalCount
        };
    }
}