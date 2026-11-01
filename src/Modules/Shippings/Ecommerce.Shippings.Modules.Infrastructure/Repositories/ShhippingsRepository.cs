using Ecommerce.Shipings.Modules.Domain.Entities;
using Ecommerce.Shipings.Modules.Infrastructure.Database;
using Ecommerce.Shippings.Modules.Domain.Repositories;
using Ecommerce.Shippings.Modules.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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
}